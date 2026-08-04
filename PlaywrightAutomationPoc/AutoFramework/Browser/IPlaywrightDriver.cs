using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IPlaywrightDriver : IAsyncDisposable
    {
        IPage Page { get; }
        Task InitializeAsync();
        // Creates a new browser context and page. Caller is responsible for closing the context when done.
        Task<IPage> CreateNewPageAsync();
    }
}