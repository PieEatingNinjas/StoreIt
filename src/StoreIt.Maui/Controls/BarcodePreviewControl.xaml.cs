using StoreIt.Maui.Helpers;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Controls;

public partial class BarcodePreviewControl : ContentView
{
    public static readonly BindableProperty BarcodeFormatProperty =
        BindableProperty.Create(nameof(BarcodeFormat), typeof(string), typeof(BarcodePreviewControl), string.Empty, propertyChanged: OnValueUpdated);

    public static readonly BindableProperty BarcodeDataProperty =
        BindableProperty.Create(nameof(BarcodeData), typeof(string), typeof(BarcodePreviewControl), string.Empty, propertyChanged: OnValueUpdated);

    public static readonly BindableProperty ShowValidationProperty =
        BindableProperty.Create(nameof(ShowValidation), typeof(bool), typeof(BarcodePreviewControl), false, propertyChanged: OnValueUpdated);


    public string BarcodeData
    {
        get => (string)GetValue(BarcodeDataProperty);
        set => SetValue(BarcodeDataProperty, value);
    }

    public string BarcodeFormat
    {
        get => (string)GetValue(BarcodeFormatProperty);
        set => SetValue(BarcodeFormatProperty, value);
    }

    public bool ShowValidation
    {
        get => (bool)GetValue(ShowValidationProperty);
        set => SetValue(ShowValidationProperty, value);
    }

    public BarcodePreviewControl()
    {
        InitializeComponent();
    }

    private static void OnValueUpdated(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BarcodePreviewControl control)
        {
            control.SetBarCode(control.BarcodeData, control.BarcodeFormat, control.ShowValidation);
        }
    }

    private BarcodeFormat GetBarcodeFormat(string format)
    {
        if (Enum.TryParse<BarcodeFormat>(format, out var barcodeFormat))
        {
            return barcodeFormat;
        }
        else
        {
            return ZXing.Net.Maui.BarcodeFormat.QrCode;
        }
    }
    
    private void SetBarCode(string data, string format, bool showValidation)
    {
        var barcodeFormat = GetBarcodeFormat(format);

        if (BarCodeHelper.IsValidBarcodeFormat(data, format))
        {
            previewBarcodeGenerator.Format = barcodeFormat;
            previewBarcodeGenerator.Value = data;
            barcodePreviewText.Text = $"{(showValidation ? "✅" : "")}{format}: {data}";
        }
        else
        {
            previewBarcodeGenerator.Format = barcodeFormat;
            previewBarcodeGenerator.Value = string.Empty;

            if(showValidation)
            {
                barcodePreviewText.Text = "❌ Ongeldig formaat";
            }
            else
            {
                barcodePreviewText.Text = string.Empty;
            }
        }
    }
}
