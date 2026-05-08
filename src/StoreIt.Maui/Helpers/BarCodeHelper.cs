namespace StoreIt.Maui.Helpers;

public static class BarCodeHelper
{
    public static List<string> GetSupportedBarcodeFormats()
    {
        return
        [
            nameof(ZXing.Net.Maui.BarcodeFormat.Code128),
            nameof(ZXing.Net.Maui.BarcodeFormat.Code39),
            nameof(ZXing.Net.Maui.BarcodeFormat.Code93),
            nameof(ZXing.Net.Maui.BarcodeFormat.Ean13),
            nameof(ZXing.Net.Maui.BarcodeFormat.Ean8),
            nameof(ZXing.Net.Maui.BarcodeFormat.UpcA),
            nameof(ZXing.Net.Maui.BarcodeFormat.UpcE),
            nameof(ZXing.Net.Maui.BarcodeFormat.Codabar),
            nameof(ZXing.Net.Maui.BarcodeFormat.Itf),
            nameof(ZXing.Net.Maui.BarcodeFormat.QrCode),
            nameof(ZXing.Net.Maui.BarcodeFormat.DataMatrix),
            nameof(ZXing.Net.Maui.BarcodeFormat.Pdf417)
        ];
    }

    public static bool IsValidBarcodeFormat(string input, string type)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (Enum.TryParse<ZXing.Net.Maui.BarcodeFormat>(type, out var format))
        {
            return format switch
            {
                ZXing.Net.Maui.BarcodeFormat.Code128 => input.Length >= 1, // Code128 can encode almost anything
                ZXing.Net.Maui.BarcodeFormat.Code39 => input.All(c =>
                    "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%".Contains(c)),
                ZXing.Net.Maui.BarcodeFormat.Code93 => input.All(c =>
                    "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%".Contains(c)),
                ZXing.Net.Maui.BarcodeFormat.Ean13 => input.All(char.IsDigit) && input.Length == 13,
                ZXing.Net.Maui.BarcodeFormat.Ean8 => input.All(char.IsDigit) && input.Length == 8,
                ZXing.Net.Maui.BarcodeFormat.UpcA => input.All(char.IsDigit) && input.Length == 12,
                ZXing.Net.Maui.BarcodeFormat.UpcE =>
                    input.All(char.IsDigit) && (input.Length == 6 || input.Length == 8),
                ZXing.Net.Maui.BarcodeFormat.Codabar => input.All(c => "0123456789-$:/.+".Contains(c)),
                ZXing.Net.Maui.BarcodeFormat.Itf => input.All(char.IsDigit) && input.Length % 2 == 0,
                ZXing.Net.Maui.BarcodeFormat.QrCode => input.Length >= 1, // QR codes are very flexible
                ZXing.Net.Maui.BarcodeFormat.DataMatrix => input.Length >= 1, // DataMatrix is very flexible
                ZXing.Net.Maui.BarcodeFormat.Pdf417 => input.Length >= 1, // PDF417 is very flexible
                _ => false
            };
        }

        return false;
    }
}