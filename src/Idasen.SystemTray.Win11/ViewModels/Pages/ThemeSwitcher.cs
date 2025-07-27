using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public class ThemeSwitcher : IThemeSwitcher
{
    public void ChangeTheme(string           parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (ApplicationThemeManager.GetAppTheme (  ) == ApplicationTheme.Light)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Light);

                break;

            case "theme_dark":
                if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Dark);

                break;

            case "theme_high_contrast":
                if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.HighContrast)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.HighContrast);

                break;

            default:
                if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Unknown)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Unknown);

                break;
        }
    }
}