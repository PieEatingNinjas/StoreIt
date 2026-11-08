namespace StoreIt.Services;

public interface IAppNavigationService : INavigationService
{
    Task NavigateToScanBarCodePage();
    Task NavigateToAddCardPage();
    /// <summary>Shows card details and completes when the sheet is dismissed.</summary>
    Task NavigateToViewCardPage(int id);

    Task<bool> ResumeCardDetailsAsync();
    Task NavigateToEditCardPage(int id);
    Task GoToRoot();
    Task Show(Type pageType);
}
