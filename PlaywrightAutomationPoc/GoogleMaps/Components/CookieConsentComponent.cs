// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public class CookieConsentComponent
    {
        private readonly IPage _page;

        // Locators for the cookie consent component
        private ILocator CookieButton => _page.GetByRole(AriaRole.Button, new() { Name = "Accept all" });

        public CookieConsentComponent(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
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