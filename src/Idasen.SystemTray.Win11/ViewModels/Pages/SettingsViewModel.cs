﻿using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Idasen.SystemTray.Win11.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized ;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        // General Settings
        [ObservableProperty]
        private uint _standing = 100;

        [ObservableProperty]
        private uint _seating = 90;

        // Advanced Settings
        [ObservableProperty]
        private string _deskName = string.Empty;

        [ObservableProperty]
        private string _deskAddress = string.Empty;

        [ObservableProperty]
        private bool _parentalLock ;

        [ObservableProperty]
        private bool _notifications = true;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"UiDesktopApp1 - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                case "theme_dark":
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;

                case "theme_high_contrast":
                    if (CurrentTheme == ApplicationTheme.HighContrast)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.HighContrast);
                    CurrentTheme = ApplicationTheme.HighContrast;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Unknown)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Unknown);
                    CurrentTheme = ApplicationTheme.Unknown;

                    break;
            }
        }
    }
}
