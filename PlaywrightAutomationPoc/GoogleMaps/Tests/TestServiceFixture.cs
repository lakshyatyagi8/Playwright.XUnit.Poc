using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.Config;
using PlaywrightAutomationPoc.GoogleMaps.Pages;
using Xunit;
using PlaywrightAutomationPoc.AutoFramework.Extensions;

namespace PlaywrightAutomationPoc.GoogleMaps.Tests
{
    public interface ITestServiceFixture : IAsyncLifetime
    {
        IServiceProvider Services { get; }
        IPlaywrightDriver PlaywrightDriver { get; }
        IMapsPage MapsPage { get; }
    }

    public class TestServiceFixture : IAsyncLifetime
    {
        private const string EnvName = "Dev";

        public IServiceProvider ServiceProvider { get; }
        public IPlaywrightDriver PlaywrightDriver { get; }
        public Lazy<Task<IMapsPage>> MapsPageTask { get; private set; }
        public IMapsPage? MapsPage { get; private set; }

        public TestServiceFixture()
        {
            var services = new ServiceCollection();
            // ✅ The crucial step: invoke your extension methods here
            services.AddCoreFramework();
            services.AddGoogleMapsPages();

            // Build the provider AFTER registering the services
            ServiceProvider = services.BuildServiceProvider();

            // Register test settings created from the environment config file
            var testSetting = new TestSettingInitializer().GetTestSettingByConfigFile(EnvName);
            services.AddSingleton<ITestSetting>(testSetting);

            // Register the PlaywrightDriver which depends on ITestSetting, IBrowserProvider, IPageFactory
            services.AddSingleton<IPlaywrightDriver, PlaywrightDriver>();

            ServiceProvider = services.BuildServiceProvider();

            PlaywrightDriver = ServiceProvider.GetRequiredService<IPlaywrightDriver>();
            
            // Create a lazy instance of MapsPage that will be initialized after PlaywrightDriver.InitializeAsync()
            MapsPageTask = new Lazy<Task<IMapsPage>>(() => 
            {
                // The synchronous result is wrapped in a Task<IMapsPage> by the Lazy class.
                return Task.FromResult<IMapsPage>(new MapsPage(PlaywrightDriver.Page));
            });
        }

        public async Task InitializeAsync()
        {
            await PlaywrightDriver.InitializeAsync();
            // Initialize MapsPage after the driver is ready            
            MapsPage = await MapsPageTask.Value; 
        }

        public async Task DisposeAsync()
        {
            if (ServiceProvider is IAsyncDisposable asyncDisp)
            {
                await asyncDisp.DisposeAsync();
            }

            if (ServiceProvider is IDisposable disp)
            {
                disp.Dispose();
            }
        }
    }
}
