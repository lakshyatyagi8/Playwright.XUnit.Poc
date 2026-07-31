// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.

using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Reporter;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public abstract class BasePage
    {
        /// <summary>
        /// The Playwright page instance used for interacting with the Google Maps web page.
        /// </summary>
        protected readonly IPage _page;        
        private readonly IReportGenerator _reporter;

        // Locators are defined as properties (Encapsulation)
        protected ILocator SearchBox => _page.Locator("input[name='q']");
        
        protected ILocator SearchButton => _page.GetByRole(AriaRole.Button, new() { Name = "Search" });
        
        protected ILocator CookieButton => _page.GetByRole(AriaRole.Button, new() { Name = "Accept all" });

        protected BasePage(IPage page, IReportGenerator reporter)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));            
        }

        /// <summary>
        /// Navigates to the specified base URL.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public async Task NavigateAsync(string baseUrl)
        {
            await _page.GotoAsync(baseUrl);
        }

        /// <summary>
        /// Handles cookie consent by clicking the accept button if it appears.
        /// </summary>
        /// <returns></returns>
        public async Task HandleCookiesAsync()
        {
            // Fail-safe: Only click if it appears (timeout reduced to avoid long waits)
            try 
            {
                if (await CookieButton.IsVisibleAsync())
                {
                    await CookieButton.ClickAsync();
                }
            } 
            catch { /* Ignore if cookie banner doesn't appear */ }
        }

    }
}