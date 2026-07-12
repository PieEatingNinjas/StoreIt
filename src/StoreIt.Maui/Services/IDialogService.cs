using System;

namespace StoreIt.Services;

public interface IDialogService
{
    Task DisplayAlert(string title, string message, string cancel);
    Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    Task<string?> DisplayActionSheet(string title, string cancel, params string[] buttons);
}