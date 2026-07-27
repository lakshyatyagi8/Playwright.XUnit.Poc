// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Extensions;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public class MapsPage : BasePage, IMapsPage
    {
        private ILocator Directions => _page.GetByRole(AriaRole.Button, new() { Name = "Directions" });

        private ILocator StartingPoint => _page.GetByPlaceholder("Choose starting point, or click on the map...");
        // Selects the first result that looks like a location link
        private ILocator FirstResult => _page.Locator("a[href*='/place/']").First; 
        private ILocator Headline(string sDestination) => _page.Locator($"div[aria-label*='{sDestination}']").First;
        private ILocator RouteOption1Time => _page.Locator("//div[@id='section-directions-trip-0']//div[contains(@class,'Fk3sm fontHeadlineSmall')]");
        
        // 2. Constructor
        // Passes the IPage instance up to the BasePage
        public MapsPage(IPage page) : base(page)
        {
            
        }

        /// <summary>
        /// Searches for a location using the search box.
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public async Task SearchLocationAsync(string locationName)
        {
            await SearchBox.FillAsync(locationName);
            await SearchButton.ClickAsync();
        }

        public new async Task NavigateAsync(string baseUrl)
        {
            await base.NavigateAsync(baseUrl);
        }

        public new async Task HandleCookiesAsync()
        {
            await base.HandleCookiesAsync();
        }

        /// <summary>
        /// Opens the directions panel.
        /// </summary>
        /// <returns></returns>
        private async Task OpenDirectionsAsync()
        {
            await Directions.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
            await Directions.ClickAsync();
        }
        
        
        public async Task SetRouteLocationsAndSearchAsync(string startLocationName, string [] arrStopLocations = null!)
        {
            if (arrStopLocations.Length < 1)
                throw new ArgumentException("At least one stop locations are required for a route.");

            await OpenDirectionsAsync();
            // Fill in starting point and destination
            await StartingPoint.SetTextValueAndEnterOnLocator(startLocationName, _page.Locator("//div[@class='KG8wXc fontBodyMedium']"));
            
            for(int i = 0; i < arrStopLocations.Length; i++)
            {
                if(i > 0)
                {
                    await _page.Locator("div[class='d2cEI'] span[class='ExQYxb google-symbols']").SafeClickAsync();
                }
                var stopLocationName = arrStopLocations[i];
                var stopPoint = _page.Locator("//input[@placeholder='Choose destination, or click on the map...']");                
                await stopPoint.SetTextValueAndEnterOnLocator(stopLocationName, _page.Locator("//div[@class='KG8wXc fontBodyMedium']"));
            }
        }

        public async Task SelectFirstResultAsync()
        {
            await FirstResult.SafeClickAsync();
        }

        public async Task<string> GetHeadlineAsync(string sDestination)
        {
            await Headline(sDestination).WaitForAsync();
            return await Headline(sDestination).InnerTextAsync();
        }
        
        public async Task<string> GetRouteOptionTimeAsync()
        {
            await RouteOption1Time.WaitForAsync();
            return await RouteOption1Time.InnerTextAsync();
        }
    }
}