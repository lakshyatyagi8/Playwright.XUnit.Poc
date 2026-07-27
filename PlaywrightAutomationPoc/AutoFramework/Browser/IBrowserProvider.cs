using Microsoft.Playwright;
using BrowserKind = PlaywrightAutomationPoc.AutoFramework.Enum.BrowserType;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IBrowserFactory
    {
        Task<IBrowser> LaunchAsync(BrowserKind browserType, BrowserTypeLaunchOptions options);
    }
}