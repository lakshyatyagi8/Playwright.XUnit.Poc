// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Config;
using BrowserKind = PlaywrightAutomationPoc.AutoFramework.Enum.BrowserType;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public class PlaywrightDriver : IPlaywrightDriver, IAsyncDisposable
    {
        private readonly IBrowserFactory _browserFactory;
        private readonly IPageFactory _pageFactory;
        private readonly PlaywrightSettings _playwrightSettings;
        private readonly BrowserConfiguration _config;
        
        private IBrowser? _browser;
        private IPage? _page;

        public PlaywrightDriver(
            IBrowserFactory browserFactory, 
            IPageFactory pageFactory,
            IOptions<PlaywrightSettings> playwrightSettings,
            IOptions<ApplicationSettings> applicationSettings)
        {
            _browserFactory = browserFactory ?? throw new ArgumentNullException(nameof(browserFactory));
            _pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
            _playwrightSettings = playwrightSettings?.Value ?? throw new ArgumentNullException(nameof(playwrightSettings));            
            // Map the options to your browser configuration builder
            _config = new BrowserConfiguration(playwrightSettings, applicationSettings);
        }

        public IPage Page => _page ?? throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");

        public async Task<IPage> CreateNewPageAsync()
        {
            if (_browser == null)
                throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");
            
            return await _pageFactory.CreatePageAsync(_browser, _config.GetBrowserNewPageOptions());
        }

        public async Task InitializeAsync()
        {
            _browser = await InitializePlaywrightBrowserAsync();
            
            var contextOptions = _config.GetBrowserNewPageOptions();
            var videoSetting = _playwrightSettings.Video?.ToLowerInvariant() ?? "off";
            if (videoSetting == "on" || videoSetting == "retain-on-failure")
            {
                contextOptions.RecordVideoDir = "videos";
                contextOptions.RecordVideoSize = new RecordVideoSize { Width = _playwrightSettings.ViewportWidth, Height = _playwrightSettings.ViewportHeight };
            }
            _page = await _pageFactory.CreatePageAsync(_browser, contextOptions);            
        }

        private async Task<IBrowser> InitializePlaywrightBrowserAsync()
        {
            var browserType = ParseBrowserType(_playwrightSettings.BrowserType);
            return await _browserFactory.LaunchAsync(browserType, _config.GetBrowserLaunchOptions());
        }

        private static BrowserKind ParseBrowserType(string browserType)
        {
            if (string.IsNullOrWhiteSpace(browserType))
            {
                throw new ArgumentException("Browser type cannot be empty.", nameof(browserType));
            }

            return System.Enum.TryParse<BrowserKind>(browserType, true, out var parsedBrowserType)
                ? parsedBrowserType
                : throw new ArgumentException($"Unsupported browser type: {browserType}", nameof(browserType));
        }

        public async ValueTask DisposeAsync()
        {            
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
            }
        }
    }
}