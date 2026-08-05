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
        
        public CookieConsentComponent CookieConsent { get; }

        protected BasePage(IPage page, IReportGenerator reporter)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            // Instantiate the components, passing the Playwright page down
            CookieConsent = new CookieConsentComponent(page);
        }

        /// <summary>
        /// Navigates to the specified base URL.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public async Task NavigateAsync(string baseUrl)
        {
            await _page.GotoAsync(baseUrl + "/maps");
        }

    }
}