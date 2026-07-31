// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.AutoFramework.Reporter;
using PlaywrightAutomationPoc.Config;
using PlaywrightAutomationPoc.GoogleMaps.Pages;

namespace PlaywrightAutomationPoc.GoogleMaps.Tests
{
    public interface ITestServiceFixture : IAsyncLifetime
    {
        IServiceProvider ServiceProvider { get; }
        IPlaywrightDriver PlaywrightDriver { get; }
    }

    /// <summary>
    /// A fixture class that sets up the test services and dependencies for Google Maps tests.
    /// It initializes the Playwright driver, configures the service provider, and provides access to the necessary services for testing.
    /// </summary>
    public class TestServiceFixture : ITestServiceFixture
    {
        private const string EnvName = "Dev";

        public IServiceProvider ServiceProvider { get; }
        public IPlaywrightDriver PlaywrightDriver { get; }        
        public IReportGenerator Reporter { get; }
        
        public MapsPage MapsPage { get; private set; } = null!;

        public TestServiceFixture()
        {
            var services = new ServiceCollection();

            // 1. Setup Configuration
            var testSetting = new TestSettingInitializer().GetTestSettingByConfigFile(EnvName);
            services.AddSingleton<ITestSetting>(testSetting);            

            // 2. Invoke framework extension methods
            services.AddCoreFramework();
            services.AddGoogleMapsPages();

            // 3. Build the provider EXACTLY ONCE
            ServiceProvider = services.BuildServiceProvider();

            // 4. Resolve the driver (DI handles the injection of IBrowserProvider & IPageFactory)
            PlaywrightDriver = ServiceProvider.GetRequiredService<IPlaywrightDriver>();
            
            Reporter = ServiceProvider.GetRequiredService<IReportGenerator>(); // Resolve Reporter
            // Initialize report once per test run
            var reportPath = Path.Combine(AppContext.BaseDirectory, "Reports", "test-report.html");
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
            Reporter.InitializeReport(reportPath);
        }

        public async Task InitializeAsync()
        {
            // Initialize the browser context asynchronously
            await PlaywrightDriver.InitializeAsync();
            await PlaywrightDriver.Page.Context.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            // Resolve the page natively from the DI container instead of using 'new'
            MapsPage = ServiceProvider.GetRequiredService<MapsPage>();
        }

        public async Task DisposeAsync()
        {
            // Flush the report to save the HTML file
            Reporter.Flush();
            // Cast to ServiceProvider for a clean, single asynchronous disposal
            if (ServiceProvider is ServiceProvider sp)
            {
                await sp.DisposeAsync();
            }
        }
    }
}