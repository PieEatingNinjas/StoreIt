namespace StoreIt.Services;

public interface INavigationService
{
    Task NavigateTo(string route, Dictionary<string, object>? parameters = null);
    Task GoBack();
    Task GoBack(Dictionary<string, object> parameters);
}