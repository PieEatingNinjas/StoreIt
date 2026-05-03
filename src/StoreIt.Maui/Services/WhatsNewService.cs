using StoreIt.Services;
using StoreIt.WhatsNew;

namespace StoreIt.Maui.Services;

public class WhatsNewService : IWhatsNewService
{
	private readonly IUserPreferencesService _userPreferencesService;
	private readonly IAppNavigationService _appNavigationService;

	public WhatsNewService(IUserPreferencesService userPreferencesService,
		IAppNavigationService appNavigationService)
	{
		_userPreferencesService = userPreferencesService;
		_appNavigationService = appNavigationService;
	}

	public Task ShowLatestWhatsNewAsync(bool bypassCheck = false)
	{
		var latestVersion = WhatsNewRegistry.Items
			.OrderByDescending(i => i.Id)
			.FirstOrDefault();

		if (latestVersion is not null
			&& (latestVersion.Id != _userPreferencesService.GetLastViewedWhatsNewId() || bypassCheck))
		{
			return _appNavigationService.Show(latestVersion.PageType);
		}
		return Task.CompletedTask;
	}
}