namespace StoreIt.Maui.Services;

public interface IWhatsNewService
{
	Task ShowLatestWhatsNewAsync(bool bypassCheck = false);
}
