// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using Microsoft.Extensions.Options;
using PlaywrightAutomationPoc.AutoFramework.Config;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IBrowserConfiguration
    {
        BrowserTypeLaunchOptions GetBrowserLaunchOptions();
        BrowserNewPageOptions GetBrowserNewPageOptions();
    }

    public class BrowserConfiguration : IBrowserConfiguration
    {
        private readonly PlaywrightSettings _playwrightSettings;
        private readonly ApplicationSettings _applicationSettings;

        public BrowserConfiguration(IOptions<PlaywrightSettings> playwrightSettingsOptions, IOptions<ApplicationSettings> applicationSettingsOptions)
        {
            ArgumentNullException.ThrowIfNull(playwrightSettingsOptions);
            ArgumentNullException.ThrowIfNull(applicationSettingsOptions);

            _playwrightSettings = playwrightSettingsOptions.Value ?? throw new ArgumentNullException(nameof(playwrightSettingsOptions.Value));
            _applicationSettings = applicationSettingsOptions.Value ?? throw new ArgumentNullException(nameof(applicationSettingsOptions.Value));
        }

        public BrowserTypeLaunchOptions GetBrowserLaunchOptions()
        {
            return new BrowserTypeLaunchOptions
            {
                SlowMo = _playwrightSettings?.SlowMo ?? 0,
                Headless = _playwrightSettings?.Headless ?? true,
                Channel = _playwrightSettings?.BrowserType
            };
        }

        public BrowserNewPageOptions GetBrowserNewPageOptions()
        {
            return new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = _playwrightSettings?.ViewportWidth ?? 1920,
                    Height = _playwrightSettings?.ViewportHeight ?? 1080
                },
                BaseURL = _applicationSettings?.BaseUrl ?? string.Empty,
            };
        }
    }    
}