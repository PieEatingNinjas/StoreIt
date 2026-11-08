using StoreIt.Maui.Helpers;
using ZXing;
using ZXing.Common;
using MauiBarcodeFormat = ZXing.Net.Maui.BarcodeFormat;

namespace StoreIt.Maui.Controls;

// Encodes the barcode itself (size-independent, exceptions caught) and draws it, so an unusual value,
// format or recycled cell size can never crash the app the way BarcodeGeneratorView can.
public class SafeBarcodeView : ContentView
{
    public static readonly BindableProperty FormatProperty =
        BindableProperty.Create(nameof(Format), typeof(MauiBarcodeFormat), typeof(SafeBarcodeView),
            MauiBarcodeFormat.QrCode, propertyChanged: OnChanged);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(SafeBarcodeView),
            string.Empty, propertyChanged: OnChanged);

    private readonly BarcodeDrawable _drawable = new();
    private readonly GraphicsView _graphicsView;

    public SafeBarcodeView()
    {
        _graphicsView = new GraphicsView { Drawable = _drawable, BackgroundColor = Colors.White };
        Content = _graphicsView;
    }

    public MauiBarcodeFormat Format
    {
        get => (MauiBarcodeFormat)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((SafeBarcodeView)bindable).Update();

    private void Update()
    {
        _drawable.Matrix = Encode(Value ?? string.Empty, Format);
        _graphicsView.Invalidate();
    }

    private static BitMatrix? Encode(string value, MauiBarcodeFormat format)
    {
        var zxingFormat = ToZXingFormat(format);
        if (zxingFormat is null || !BarCodeHelper.IsValidBarcodeFormat(value, format.ToString()))
        {
            return null;
        }

        try
        {
            var hints = new Dictionary<EncodeHintType, object> { [EncodeHintType.MARGIN] = 0 };
            return new MultiFormatWriter().encode(value, zxingFormat.Value, 0, 0, hints);
        }
        catch (Exception)
        {
            // Value not encodable in this format (e.g. checksum or capacity); show nothing instead of crashing.
            return null;
        }
    }

    private static BarcodeFormat? ToZXingFormat(MauiBarcodeFormat format) => format switch
    {
        MauiBarcodeFormat.QrCode => BarcodeFormat.QR_CODE,
        MauiBarcodeFormat.DataMatrix => BarcodeFormat.DATA_MATRIX,
        MauiBarcodeFormat.Pdf417 => BarcodeFormat.PDF_417,
        MauiBarcodeFormat.Code128 => BarcodeFormat.CODE_128,
        MauiBarcodeFormat.Code39 => BarcodeFormat.CODE_39,
        MauiBarcodeFormat.Code93 => BarcodeFormat.CODE_93,
        MauiBarcodeFormat.Ean13 => BarcodeFormat.EAN_13,
        MauiBarcodeFormat.Ean8 => BarcodeFormat.EAN_8,
        MauiBarcodeFormat.UpcA => BarcodeFormat.UPC_A,
        MauiBarcodeFormat.UpcE => BarcodeFormat.UPC_E,
        MauiBarcodeFormat.Codabar => BarcodeFormat.CODABAR,
        MauiBarcodeFormat.Itf => BarcodeFormat.ITF,
        _ => null
    };
}
