using System;

namespace StoreIt.Maui.Helpers;

public static class BarCodeHelper
{

    public static List<string> GetSupportedBarcodeFormats()
    {
        return
        [
            "Code128",
            "Code39",
            "Code93",
            "EAN13",
            "EAN8",
            "UPC_A",
            "UPC_E",
            "Codabar",
            "ITF",
            "QR_CODE",
            "DataMatrix",
            "PDF417"
        ];
    }
    public static bool IsValidBarcodeFormat(string input, string type)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return type switch
        {
            "Code128" => input.Length >= 1, // Code128 can encode almost anything
            "Code39" => input.All(c => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%".Contains(c)),
            "Code93" => input.All(c => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%".Contains(c)),
            "EAN13" => input.All(char.IsDigit) && input.Length == 13,
            "EAN8" => input.All(char.IsDigit) && input.Length == 8,
            "UPC_A" => input.All(char.IsDigit) && input.Length == 12,
            "UPC_E" => input.All(char.IsDigit) && (input.Length == 6 || input.Length == 8),
            "Codabar" => input.All(c => "0123456789-$:/.+".Contains(c)),
            "ITF" => input.All(char.IsDigit) && input.Length % 2 == 0,
            "QR_CODE" => input.Length >= 1, // QR codes are very flexible
            "DataMatrix" => input.Length >= 1, // DataMatrix is very flexible
            "PDF417" => input.Length >= 1, // PDF417 is very flexible
            _ => true
        };
    }
}
