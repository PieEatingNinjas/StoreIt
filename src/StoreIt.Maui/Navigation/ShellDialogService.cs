using StoreIt.Services;

namespace StoreIt.Navigation;

public class ShellDialogService : IDialogService
{
    public Task DisplayAlert(string title, string message, string cancel)
        => Shell.Current.DisplayAlertAsync(title, message, cancel);

    public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        => Shell.Current.DisplayAlertAsync(title, message, accept, cancel);

    public Task<string?> DisplayActionSheet(string title, string cancel, params string[] buttons)
        => Shell.Current.DisplayActionSheetAsync(title, cancel, null, buttons);
}