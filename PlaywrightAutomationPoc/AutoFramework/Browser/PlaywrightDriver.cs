using Microsoft.Playwright;
using PlaywrightAutomationPoc.Config;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IPlaywrightDriver : IAsyncDisposable
    {
        IPage Page { get; }
        Task InitializeAsync();
    }

    public class PlaywrightDriver : IPlaywrightDriver
    {
        /// <summary>
        /// The browser provider used to launch different browsers.
        /// </summary>
        private readonly Lazy<Task<IBrowserProvider>> _browserProviderTask;
        // 2. A private field to cache the fully initialized result
        private IBrowserProvider? _browserProvider;
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
        /// <param name="testSetting"></param>
        /// <param name="browserProvider"></param>
        /// <param name="pageFactory"></param>
        /// <exception cref="ArgumentNullException"></exception> <summary>
        public PlaywrightDriver(ITestSetting testSettingTask, IBrowserProvider browserProviderTask, IPageFactory pageFactoryTask)
        {
            _browserProviderTask = new Lazy<Task<IBrowserProvider>>(() => Task.FromResult(browserProviderTask)) ?? throw new ArgumentNullException(nameof(browserProviderTask));
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
            
            // await _page.Context.Tracing.StartAsync(new TracingStartOptions
            // {
            //     Title = $"{WithTestNameAttribute.CurrentClassName}.{WithTestNameAttribute.CurrentTestName}",
            //     Screenshots = true,
            //     Snapshots = true,
            //     Sources = true
            // });
        }

        private async Task<IBrowser> InitializePlaywrightBrowserAsync()
        {
            _browserProvider = await _browserProviderTask.Value;
            if (_testSetting == null)
                throw new InvalidOperationException("Test settings not initialized.");
            return _testSetting.BrowserType.Trim().ToLower() switch
            {
                "chromium" => await _browserProvider.LaunchChromiumBrowserAsync(_config.GetBrowserLaunchOptions()),
                "chrome" => await _browserProvider.LaunchChromeBrowserAsync(_config.GetBrowserLaunchOptions()),
                "firefox" => await _browserProvider.LaunchFirefoxBrowserAsync(_config.GetBrowserLaunchOptions()),
                "webkit" => await _browserProvider.LaunchWebkitBrowserAsync(_config.GetBrowserLaunchOptions()),
                "msedge" => await _browserProvider.LaunchEdgeBrowserAsync(_config.GetBrowserLaunchOptions()),
                _ => throw new ArgumentException($"Unsupported browser channel: {_testSetting.BrowserType}"),
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (_page != null)
            {
                await _page.Context.Tracing.StopAsync(new()
                {
                    Path = Path.Combine(
                        Environment.CurrentDirectory,
                        "playwright-traces",
                    $"{WithTestNameAttribute.CurrentClassName}.{WithTestNameAttribute.CurrentTestName}.zip"
                    )
                });
            }
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
            }
        }
    }
}