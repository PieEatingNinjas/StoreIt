using StoreIt.Maui.Helpers;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Controls;

public partial class BarcodePreviewControl : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty BarcodeFormatProperty =
        BindableProperty.Create(nameof(BarcodeFormat), typeof(string), typeof(BarcodePreviewControl), string.Empty, propertyChanged: OnValueUpdated);

    public static readonly BindableProperty BarcodeDataProperty =
        BindableProperty.Create(nameof(BarcodeData), typeof(string), typeof(BarcodePreviewControl), string.Empty, propertyChanged: OnValueUpdated);

    // public static readonly BindableProperty FontSizeProperty =
    //     BindableProperty.Create(nameof(FontSize), typeof(double), typeof(HintControl), 14.0);

    public static readonly BindableProperty ShowValidationProperty =
        BindableProperty.Create(nameof(ShowValidation), typeof(bool), typeof(BarcodePreviewControl), false, propertyChanged: OnValueUpdated);

    // public static readonly BindableProperty ShowDurationProperty =
    //     BindableProperty.Create(nameof(ShowDuration), typeof(int), typeof(HintControl), 4000);

    // public static readonly BindableProperty FadeAnimationDurationProperty =
    //     BindableProperty.Create(nameof(FadeAnimationDuration), typeof(uint), typeof(HintControl), 500u);

    #endregion

    #region Properties

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

    // public double FontSize
    // {
    //     get => (double)GetValue(FontSizeProperty);
    //     set => SetValue(FontSizeProperty, value);
    // }

    public bool ShowValidation
    {
        get => (bool)GetValue(ShowValidationProperty);
        set => SetValue(ShowValidationProperty, value);
    }

    // public int ShowDuration
    // {
    //     get => (int)GetValue(ShowDurationProperty);
    //     set => SetValue(ShowDurationProperty, value);
    // }

    // public uint FadeAnimationDuration
    // {
    //     get => (uint)GetValue(FadeAnimationDurationProperty);
    //     set => SetValue(FadeAnimationDurationProperty, value);
    // }

    #endregion

    public BarcodePreviewControl()
    {
        InitializeComponent();
        // hintBorder.Opacity = 0;
        // hintBorder.IsVisible = false;
    }

    #region Methods

    // public async Task ShowHintBrieflyAsync()
    // {
    //     IsVisible = true;

    //     // Wait for the specified duration, then hide the hint
    //     await Task.Delay(ShowDuration);
    //     IsVisible = false;
    // }
    // #endregion

    // #region Event Handlers

    private static async void OnValueUpdated(BindableObject bindable, object oldValue, object newValue)
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

    #endregion
}
