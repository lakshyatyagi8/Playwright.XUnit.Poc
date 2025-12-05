using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public interface IPageFactory
    {
        Task<IPage> CreatePageAsync(IBrowser browser, BrowserNewPageOptions newPageOptions);
    }

    public class PageFactory : IPageFactory
    {
        public async Task<IPage> CreatePageAsync(IBrowser browser, BrowserNewPageOptions newPageOptions)
        {
            return await browser.NewPageAsync(
                new BrowserNewPageOptions
                {
                    ViewportSize = newPageOptions.ViewportSize,
                    BaseURL = newPageOptions.BaseURL
                }
            );
        }
    }
}