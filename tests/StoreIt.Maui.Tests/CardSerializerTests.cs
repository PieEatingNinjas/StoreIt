using System.IO.Compression;
using System.Text;
using FluentAssertions;
using StoreIt.Sharing.Models;
using StoreIt.Sharing.Services;
using Xunit;

namespace StoreIt.Maui.Tests;

public sealed class CardSerializerTests
{
    private readonly CardSerializer _sut = new();

    [Fact]
    public void CreateLink_ValidCard_RoundTripsSharedFields()
    {
        var card = CreateCustomCard("Delhaize", isFavorite: true, isPrivate: true);

        var link = _sut.CreateLink(card);
        var actual = _sut.Deserialize(link);

        link.Scheme.Should().Be("storeit");
        link.Host.Should().Be("import");
        actual.Should().ContainSingle().Which.Should().Be(card);
    }

    [Fact]
    public void CreateLink_LegacyCardWithoutColor_RoundTripsCard()
    {
        var card = CreateCustomCard("Legacy") with { Color = null };

        _sut.Deserialize(_sut.CreateLink(card)).Should().ContainSingle().Which.Should().Be(card);
    }

    [Fact]
    public void CreateLink_ValidBarcode_RoundTripsBarcode()
    {
        var card = new CardShareData(
            "Albert Heijn",
            "Klantenkaart",
            "5901234123457",
            "Ean13",
            null,
            "#2196F3",
            false,
            false);

        _sut.Deserialize(_sut.CreateLink(card)).Should().ContainSingle().Which.Should().Be(card);
    }

    [Theory]
    [InlineData("https://example.com/?d=abc")]
    [InlineData("storeit://other?d=abc")]
    [InlineData("storeit://import")]
    [InlineData("storeit://import?d=abc&extra=value")]
    public void Deserialize_InvalidLink_ThrowsInvalidPayload(string value)
    {
        var act = () => _sut.Deserialize(new Uri(value));

        act.Should().Throw<InvalidCardPayloadException>();
    }

    [Fact]
    public void Deserialize_UnsupportedVersion_ThrowsDedicatedException()
    {
        var futureCard = ValidCardJson().Insert(1, "\"futureField\":true,");
        var payload = "{\"v\":2,\"cards\":[" + futureCard + "],\"futureEnvelopeField\":true}";
        var link = CreateLink(payload);

        var act = () => _sut.Deserialize(link);

        act.Should().Throw<UnsupportedCardVersionException>();
    }

    [Fact]
    public void Deserialize_UnknownProperty_RejectsPayload()
    {
        var card = ValidCardJson().Insert(1, "\"unexpected\":true,");
        var link = CreateLink("{\"v\":1,\"cards\":[" + card + "]}");

        var act = () => _sut.Deserialize(link);

        act.Should().Throw<InvalidCardPayloadException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("Unknown")]
    public void CreateLink_InvalidBarcodeFormat_RejectsCard(string format)
    {
        var card = new CardShareData("Test", string.Empty, "123", format, null, "#FF6B35", false, false);

        var act = () => _sut.CreateLink(card);

        act.Should().Throw<InvalidCardPayloadException>();
    }

    [Fact]
    public void CreateLink_BarcodeAndCustomCode_RejectsCard()
    {
        var card = new CardShareData("Test", string.Empty, "123", "Code128", "123", "#FF6B35", false, false);

        var act = () => _sut.CreateLink(card);

        act.Should().Throw<InvalidCardPayloadException>();
    }

    [Fact]
    public void Deserialize_OversizedDecompressedPayload_RejectsWithoutImporting()
    {
        var cards = Enumerable.Range(0, 20)
            .Select(index => ValidCardJson(new string('x', 4096), index));
        var link = CreateLink("{\"v\":1,\"cards\":[" + string.Join(',', cards) + "]}");

        var act = () => _sut.Deserialize(link);

        act.Should().Throw<InvalidCardPayloadException>();
    }

    private static CardShareData CreateCustomCard(string name, bool isFavorite = false, bool isPrivate = false) =>
        new(name, "Loyalty", null, null, "ABC-123", "#FF6B35", isFavorite, isPrivate);

    private static string ValidCardJson(string customCode = "ABC-123", int index = 0) =>
        $"{{\"name\":\"Card {index}\",\"description\":\"Test\",\"barcodeData\":null," +
        $"\"barcodeFormat\":null,\"customCode\":\"{customCode}\",\"color\":\"#FF6B35\"," +
        "\"isFavorite\":false,\"isPrivate\":false}";

    private static Uri CreateLink(string json)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            gzip.Write(Encoding.UTF8.GetBytes(json));
        }

        var encoded = Convert.ToBase64String(output.ToArray())
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
        return new Uri($"storeit://import?d={encoded}");
    }
}
