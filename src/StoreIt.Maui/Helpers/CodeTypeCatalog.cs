using StoreIt.Maui.Models;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Helpers;

public static class CodeTypeCatalog
{
    public const string TextFormat = "Text";

    // Returns fresh instances: options carry per-screen state (selection, preview).
    public static List<CodeTypeOption> CreateOptions() =>
    [
        new() { Format = TextFormat, Title = "Alleen tekst", Subtitle = "Geen barcode. Ideaal voor toegangs- of pincodes", SampleValue = "ABC-123", BarcodeFormat = BarcodeFormat.QrCode },
        Barcode(BarcodeFormat.QrCode, "QR-code", "Vierkante code, past bij bijna alles", "StoreIt", isSquare: true),
        Barcode(BarcodeFormat.Code128, "Code 128", "Gangbaar op klantenkaarten (letters en cijfers)", "ABC123456"),
        Barcode(BarcodeFormat.Ean13, "EAN-13", "Winkelbarcode met 13 cijfers", "5901234123457"),
        Barcode(BarcodeFormat.Ean8, "EAN-8", "Korte winkelbarcode met 8 cijfers", "96385074"),
        Barcode(BarcodeFormat.Code39, "Code 39", "Hoofdletters en cijfers, vaak op lidkaarten", "ABC123"),
        Barcode(BarcodeFormat.UpcA, "UPC-A", "Amerikaanse winkelbarcode met 12 cijfers", "036000291452"),
        Barcode(BarcodeFormat.UpcE, "UPC-E", "Korte Amerikaanse winkelbarcode", "01234565"),
        Barcode(BarcodeFormat.Code93, "Code 93", "Compacte barcode met hoofdletters en cijfers", "ABC123"),
        Barcode(BarcodeFormat.Codabar, "Codabar", "Bibliotheek- en lidkaarten", "123456"),
        Barcode(BarcodeFormat.Itf, "ITF", "Even aantal cijfers, vaak op verpakkingen", "12345678"),
        Barcode(BarcodeFormat.DataMatrix, "Data Matrix", "Kleine vierkante code", "StoreIt", isSquare: true),
        Barcode(BarcodeFormat.Pdf417, "PDF417", "Brede gestapelde code, bv. tickets", "StoreIt")
    ];

    public static CodeTypeOption CreateUnknown(string format) =>
        Enum.TryParse<BarcodeFormat>(format, out var barcodeFormat)
            ? Barcode(barcodeFormat, format, "Herkend formaat", string.Empty)
            : new() { Format = format, Title = format, Subtitle = "Herkend formaat", BarcodeFormat = BarcodeFormat.QrCode };

    private static CodeTypeOption Barcode(BarcodeFormat format, string title, string subtitle, string sample,
        bool isSquare = false) =>
        new()
        {
            Format = format.ToString(),
            Title = title,
            Subtitle = subtitle,
            IsBarcode = true,
            IsSquare = isSquare,
            BarcodeFormat = format,
            SampleValue = sample
        };
}
