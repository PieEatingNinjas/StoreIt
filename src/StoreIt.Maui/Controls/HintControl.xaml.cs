namespace StoreIt.Maui.Controls;

public partial class HintControl : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(HintControl), string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(HintControl), "💡");

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(HintControl), 14.0);

    public static readonly BindableProperty IsShowingProperty =
        BindableProperty.Create(nameof(IsShowing), typeof(bool), typeof(HintControl), false, propertyChanged: OnIsShowingChanged);

    public static readonly BindableProperty ShowDurationProperty =
        BindableProperty.Create(nameof(ShowDuration), typeof(int), typeof(HintControl), 4000);

    public static readonly BindableProperty FadeAnimationDurationProperty =
        BindableProperty.Create(nameof(FadeAnimationDuration), typeof(uint), typeof(HintControl), 500u);


    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public bool IsShowing
    {
        get => (bool)GetValue(IsShowingProperty);
        set => SetValue(IsShowingProperty, value);
    }

    public int ShowDuration
    {
        get => (int)GetValue(ShowDurationProperty);
        set => SetValue(ShowDurationProperty, value);
    }

    public uint FadeAnimationDuration
    {
        get => (uint)GetValue(FadeAnimationDurationProperty);
        set => SetValue(FadeAnimationDurationProperty, value);
    }

    public HintControl()
    {
        InitializeComponent();
        hintBorder.Opacity = 0;
        hintBorder.IsVisible = false;
    }

    public async Task ShowHintBrieflyAsync()
    {
        IsVisible = true;

        // Wait for the specified duration, then hide the hint
        await Task.Delay(ShowDuration);
        IsVisible = false;
    }

    private static async void OnIsShowingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HintControl control && control.hintBorder != null)
        {
            bool isShowing = (bool)newValue;

            if (isShowing)
            {
                await control.ShowWithAnimationAsync();
                await Task.Delay(control.ShowDuration);
                await control.HideWithAnimationAsync();
            }
        }
    }

    private async Task ShowWithAnimationAsync()
    {
        if (hintBorder != null)
        {
            hintBorder.Opacity = 0;
            hintBorder.IsVisible = true;
            await hintBorder.FadeTo(1, FadeAnimationDuration);
        }
    }

    private async Task HideWithAnimationAsync()
    {
        if (hintBorder is not null && hintBorder.IsVisible)
        {
            await hintBorder.FadeTo(0, FadeAnimationDuration);
            hintBorder.IsVisible = false;
            IsShowing = false; 
        }
    }
}
