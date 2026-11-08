using Microsoft.Extensions.Logging;
using Plugin.Maui.BottomSheet.Navigation;
using StoreIt.Maui.Views;
using StoreIt.Navigation;

namespace StoreIt.Services;

public class ShellNavigationService : IAppNavigationService
{
    private readonly IBottomSheetNavigationService _bottomSheetNavigation;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ShellNavigationService> _logger;
    private int? _cardToResume;

    public ShellNavigationService(IBottomSheetNavigationService bottomSheetNavigation, IServiceProvider serviceProvider,
        ILogger<ShellNavigationService> logger)
    {
        _bottomSheetNavigation = bottomSheetNavigation;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task GoBack()
    {
        if (_bottomSheetNavigation.NavigationStack().Count > 0)
        {
            EnsureSuccess(await _bottomSheetNavigation.GoBackAsync());
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    public Task NavigateToScanBarCodePage()
        => NavigateTo(Pages.ScanBarCodePage);

    public Task NavigateTo(string route, Dictionary<string, object>? parameters = null)
    {
        if (parameters is not null)
        {
            return Shell.Current.GoToAsync(route, parameters);
        }
        else
        {
            return Shell.Current.GoToAsync(route);
        }
    }

    public Task NavigateToAddCardPage()
        => NavigateTo(Pages.AddCardPage);

    public async Task NavigateToViewCardPage(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        var sheet = _serviceProvider.GetRequiredService<ViewCardPage>();
        try
        {
            EnsureSuccess(await _bottomSheetNavigation.NavigateToAsync(sheet,
                parameters: new BottomSheetNavigationParameters { { NavigationParams.CardId, id } }));
            // Authentication can dismiss the sheet, so its navigation stack must be ready first.
            await sheet.InitializeAsync();
            await sheet.Dismissed;
        }
        finally
        {
            try
            {
                if (_bottomSheetNavigation.NavigationStack().Contains(sheet))
                {
                    EnsureSuccess(await _bottomSheetNavigation.GoBackAsync());
                }
                else if (!sheet.Dismissed.IsCompleted)
                {
                    sheet.Handler?.DisconnectHandler();
                }
            }
            finally
            {
                sheet.Release();
                sheet.Parent = null;
            }
        }
    }

    public async Task<bool> ResumeCardDetailsAsync()
    {
        if (_cardToResume is not int id)
        {
            return false;
        }

        _cardToResume = null;
#if IOS
        // OnAppearing precedes the end of a native back transition on iOS.
        var controller = Platform.GetCurrentUIViewController()
            ?? throw new InvalidOperationException("De actieve iOS-weergave kon niet gevonden worden.");
        var transition = UIKit.TransitionCoordinator_UIViewController.GetTransitionCoordinator(controller);
        if (transition is not null)
        {
            var completed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (transition.AnimateAlongsideTransition(_ => { },
                context => completed.TrySetResult(!context.IsCancelled)) && !await completed.Task)
            {
                _cardToResume = id;
                return false;
            }
        }
#endif
        await NavigateToViewCardPage(id);
        return true;
    }

    public Task GoBack(Dictionary<string, object> parameters)
        => Shell.Current.GoToAsync("..", parameters);

    public async Task NavigateToEditCardPage(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        var wasViewingCard = _bottomSheetNavigation.NavigationStack().Count > 0;
        if (wasViewingCard)
        {
            await GoBack();
            _cardToResume = id;
        }

        try
        {
            await NavigateTo(Pages.AddCardPage, new Dictionary<string, object>
            {
                { NavigationParams.CardId, id }
            });
        }
        catch
        {
            _cardToResume = null;
            throw;
        }
    }

    private void EnsureSuccess(INavigationResult result)
    {
        if (!result.Success || result.Cancelled)
        {
            _logger.LogError(result.Exception, "Card bottom-sheet navigation failed. Cancelled: {Cancelled}", result.Cancelled);
            throw new InvalidOperationException("De kaartweergave kon niet geopend of gesloten worden.", result.Exception);
        }
    }

    public Task GoToRoot()
        => NavigateTo($"/{Pages.MainPage}");

    public async Task Show(Type pageType)
    {
        Routing.RegisterRoute("temp", pageType);
        await Shell.Current.GoToAsync("temp");
        Routing.UnRegisterRoute("temp");
    }
}
