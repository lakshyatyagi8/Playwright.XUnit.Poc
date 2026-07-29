using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IPlaywrightDriver : IAsyncDisposable
    {
        IPage Page { get; }
        Task InitializeAsync();
    }
}