using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using StoreIt.Sharing.Models;

namespace StoreIt.Sharing.Services;

public sealed class CardSerializer : ICardSerializer
{
    private const int CurrentVersion = 1;
    private const int MaxEncodedPayloadLength = 8192;
    private const int MaxJsonLength = 65536;
    private const int MaxCards = 20;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        MaxDepth = 16,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public Uri CreateLink(CardShareData card)
    {
        ArgumentNullException.ThrowIfNull(card);
        Validate(card);
        var json = JsonSerializer.SerializeToUtf8Bytes(new CardEnvelope(CurrentVersion, [card]), JsonOptions);
        if (json.Length > MaxJsonLength)
        {
            throw new InvalidCardPayloadException("De kaart bevat te veel gegevens om te delen.");
        }

        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            gzip.Write(json);
        }

        var encoded = ToBase64Url(output.ToArray());
        if (encoded.Length > MaxEncodedPayloadLength)
        {
            throw new InvalidCardPayloadException("De kaart is te groot om veilig te delen.");
        }

        return new Uri($"storeit://import?d={encoded}", UriKind.Absolute);
    }

    public IReadOnlyList<CardShareData> Deserialize(Uri link)
    {
        ArgumentNullException.ThrowIfNull(link);
        ValidateLink(link);
        var query = link.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
        var encoded = Uri.UnescapeDataString(query[2..]);
        if (encoded.Length == 0 || encoded.Length > MaxEncodedPayloadLength)
        {
            throw new InvalidCardPayloadException("De StoreIt-link heeft een ongeldige grootte.");
        }

        try
        {
            var json = DecompressBounded(FromBase64Url(encoded));
            using (var document = JsonDocument.Parse(json))
            {
                if (!document.RootElement.TryGetProperty("v", out var versionElement) ||
                    !versionElement.TryGetInt32(out var version))
                {
                    throw new InvalidCardPayloadException("De StoreIt-link heeft geen geldige versie.");
                }

                if (version != CurrentVersion)
                {
                    throw new UnsupportedCardVersionException(version);
                }
            }

            var envelope = JsonSerializer.Deserialize<CardEnvelope>(json, JsonOptions)
                ?? throw new InvalidCardPayloadException("De StoreIt-link bevat geen kaarten.");
            if (envelope.Cards is not { Count: > 0 } || envelope.Cards.Count > MaxCards)
            {
                throw new InvalidCardPayloadException("De StoreIt-link bevat een ongeldig aantal kaarten.");
            }

            foreach (var card in envelope.Cards)
            {
                Validate(card);
            }

            return envelope.Cards;
        }
        catch (UnsupportedCardVersionException)
        {
            throw;
        }
        catch (InvalidCardPayloadException)
        {
            throw;
        }
        catch (Exception ex) when (ex is FormatException or IOException or JsonException or NotSupportedException)
        {
            throw new InvalidCardPayloadException("De StoreIt-link bevat beschadigde gegevens.", ex);
        }
    }

    private static void ValidateLink(Uri link)
    {
        if (!link.IsAbsoluteUri ||
            !string.Equals(link.Scheme, "storeit", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(link.Host, "import", StringComparison.OrdinalIgnoreCase) ||
            (link.AbsolutePath != string.Empty && link.AbsolutePath != "/"))
        {
            throw new InvalidCardPayloadException("Dit is geen geldige StoreIt-link.");
        }

        var query = link.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
        if (!query.StartsWith("d=", StringComparison.Ordinal) || query.Contains('&'))
        {
            throw new InvalidCardPayloadException("De StoreIt-link bevat geen geldige payload.");
        }
    }

    private static void Validate(CardShareData card)
    {
        if (card is null ||
            string.IsNullOrWhiteSpace(card.Name) || card.Name.Length > 100 ||
            card.Description?.Length > 200 ||
            card.BarcodeData?.Length > 4096 ||
            card.BarcodeFormat?.Length > 100 ||
            card.CustomCode?.Length > 4096 ||
            !IsValidColor(card.Color))
        {
            throw new InvalidCardPayloadException("De StoreIt-link bevat een ongeldige kaart.");
        }

        var hasBarcodeData = !string.IsNullOrWhiteSpace(card.BarcodeData);
        var hasBarcodeFormat = !string.IsNullOrWhiteSpace(card.BarcodeFormat);
        if (hasBarcodeData != hasBarcodeFormat)
        {
            throw new InvalidCardPayloadException("De barcodegegevens van de kaart zijn onvolledig.");
        }

        if (hasBarcodeData && !IsValidBarcode(card.BarcodeData!, card.BarcodeFormat!))
        {
            throw new InvalidCardPayloadException("De barcode van de kaart is ongeldig.");
        }

        if (hasBarcodeData == !string.IsNullOrWhiteSpace(card.CustomCode))
        {
            throw new InvalidCardPayloadException("Een kaart moet precies één barcode of code bevatten.");
        }
    }

    private static bool IsValidBarcode(string value, string format) => format switch
    {
        "Code128" or "QrCode" or "DataMatrix" or "Pdf417" => value.Length > 0,
        "Code39" or "Code93" => value.All(character =>
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%".Contains(character)),
        "Ean13" => value.Length == 13 && value.All(char.IsDigit),
        "Ean8" => value.Length == 8 && value.All(char.IsDigit),
        "UpcA" => value.Length == 12 && value.All(char.IsDigit),
        "UpcE" => (value.Length is 6 or 8) && value.All(char.IsDigit),
        "Codabar" => value.All(character => "0123456789-$:/.+".Contains(character)),
        "Itf" => value.Length > 0 && value.Length % 2 == 0 && value.All(char.IsDigit),
        _ => false
    };

    private static bool IsValidColor(string? color) =>
        color is null ||
        ((color.Length is 7 or 9) &&
         color[0] == '#' &&
         color[1..].All(Uri.IsHexDigit));

    private static byte[] DecompressBounded(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        Span<byte> buffer = stackalloc byte[4096];
        int read;
        while ((read = gzip.Read(buffer)) > 0)
        {
            if (output.Length + read > MaxJsonLength)
            {
                throw new InvalidCardPayloadException("De StoreIt-link unpackt tot te veel gegevens.");
            }

            output.Write(buffer[..read]);
        }

        return output.ToArray();
    }

    private static string ToBase64Url(byte[] value) =>
        Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] FromBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + ((4 - base64.Length % 4) % 4), '=');
        return Convert.FromBase64String(base64);
    }

    private sealed record CardEnvelope(
        [property: JsonPropertyName("v")] int Version,
        [property: JsonPropertyName("cards")] IReadOnlyList<CardShareData> Cards);
}
