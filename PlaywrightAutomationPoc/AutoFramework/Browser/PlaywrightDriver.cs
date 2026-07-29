// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using BrowserKind = PlaywrightAutomationPoc.AutoFramework.Enum.BrowserType;
using PlaywrightAutomationPoc.Config;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public class PlaywrightDriver : IPlaywrightDriver
    {
        /// <summary>
        /// The browser factory used to launch different browsers.
        /// </summary>
        private readonly Lazy<Task<IBrowserFactory>> _browserFactoryTask;
        // 2. A private field to cache the fully initialized result
        private IBrowserFactory? _browserFactory;
        /// <summary>
        /// The page factory used to create Playwright pages.
        /// </summary>
        private readonly Lazy<Task<IPageFactory>> _pageFactoryTask;
        // 2. A private field to cache the fully initialized result
        private IPageFactory? _pageFactory;
        /// <summary>
        ///    The test settings containing browser type and other configurations.
        /// </summary>
        private readonly Lazy<Task<ITestSetting>> _testSettingTask;        
        // 2. A private field to cache the fully initialized result
        private ITestSetting? _testSetting;
        
        /// <summary>
        /// The browser configuration settings.
        /// </summary>
        private BrowserConfiguration _config;
        /// <summary>
        /// The Playwright browser instance.
        /// </summary>
        private IBrowser? _browser;
        /// <summary>
        /// The Playwright page instance.
        /// </summary>
        private IPage? _page;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaywrightDriver"/> class.
        /// </summary>
        /// <param name="testSetting">Test settings containing browser type and other configurations.</param>
        /// <param name="browserFactory">Factory for launching different browsers.</param>
        /// <param name="pageFactory">Factory for creating Playwright pages.</param>
        /// <exception cref="ArgumentNullException"></exception> <summary>
        public PlaywrightDriver(ITestSetting testSettingTask, IBrowserFactory browserFactoryTask, IPageFactory pageFactoryTask)
        {
            _browserFactoryTask = new Lazy<Task<IBrowserFactory>>(() => Task.FromResult(browserFactoryTask)) ?? throw new ArgumentNullException(nameof(browserFactoryTask));
            _pageFactoryTask = new Lazy<Task<IPageFactory>>(() => Task.FromResult(pageFactoryTask)) ?? throw new ArgumentNullException(nameof(pageFactoryTask));
            _testSettingTask = new Lazy<Task<ITestSetting>>(() => Task.FromResult(testSettingTask)) ?? throw new ArgumentNullException(nameof(testSettingTask));
            _testSetting = _testSettingTask.Value.Result;
            _config = new BrowserConfiguration(_testSetting);
        }

        /// <summary>
        /// Gets the initialized Playwright page.
        /// </summary> <summary>
        public IPage Page => _page ?? throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");

        /// <summary>
        /// Initializes the Playwright browser and page asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            _browser = await InitializePlaywrightBrowserAsync();
            _pageFactory = await _pageFactoryTask.Value;
            _page = await _pageFactory.CreatePageAsync(_browser, _config.GetBrowserNewPageOptions());            
        }

        private async Task<IBrowser> InitializePlaywrightBrowserAsync()
        {
            _browserFactory = await _browserFactoryTask.Value;
            if (_testSetting == null)
                throw new InvalidOperationException("Test settings not initialized.");

            var browserType = ParseBrowserType(_testSetting.BrowserType);
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
                : throw new ArgumentException($"Unsupported browser channel: {browserType}", nameof(browserType));
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