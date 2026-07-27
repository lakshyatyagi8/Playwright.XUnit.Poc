// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using PlaywrightAutomationPoc.Config;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IBrowserConfiguration
    {
        BrowserTypeLaunchOptions GetBrowserLaunchOptions();
        BrowserNewPageOptions GetBrowserNewPageOptions();
    }

    public class BrowserConfiguration : IBrowserConfiguration
    {
        private readonly ITestSetting _testSetting;

        public BrowserConfiguration(ITestSetting testSetting)
        {
            _testSetting = testSetting;
        }

        public BrowserTypeLaunchOptions GetBrowserLaunchOptions()
        {
            return new BrowserTypeLaunchOptions
            {
                SlowMo = _testSetting?.SlowMo ?? 0,
                Headless = _testSetting?.Headless ?? true,
                Channel = _testSetting?.BrowserType,
                //Devtools = _testSetting?.Devtools ?? false
            };
        }

        public BrowserNewPageOptions GetBrowserNewPageOptions()
        {
            return new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = _testSetting?.Viewport?.Width ?? 1920,
                    Height = _testSetting?.Viewport?.Height ?? 1080
                },
                BaseURL = _testSetting?.BaseUrl
            };
        }
    }    
}