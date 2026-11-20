using Foundation;
using UIKit;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace StoreIt.Maui;

[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
    public override void WillConnect(
        UIScene scene,
        UISceneSession session,
        UISceneConnectionOptions connectionOptions)
    {
        base.WillConnect(scene, session, connectionOptions);
        foreach (var urlContext in connectionOptions.UrlContexts ?? [])
        {
            HandleIncomingUrl(urlContext.Url);
        }
    }

    [Export("scene:openURLContexts:")]
    public void OpenUrlContexts(UIScene scene, NSSet<UIOpenUrlContext> urlContexts)
    {
        foreach (var urlContext in urlContexts)
        {
            HandleIncomingUrl(urlContext.Url);
        }
    }

    private static void HandleIncomingUrl(NSUrl? url)
    {
        if (url?.AbsoluteString is { } value && Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            _ = ForwardIncomingLinkAsync(uri);
        }
    }

    private static async Task ForwardIncomingLinkAsync(Uri uri)
    {
        for (var attempt = 0; attempt < 50; attempt++)
        {
            if (MauiApplication.Current is { } application)
            {
                application.SendOnAppLinkRequestReceived(uri);
                return;
            }

            await Task.Delay(100);
        }
    }
}
