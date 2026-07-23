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
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = newPageOptions.ViewportSize,
                BaseURL = newPageOptions.BaseURL
            });

            return await context.NewPageAsync();
        }
    }
}