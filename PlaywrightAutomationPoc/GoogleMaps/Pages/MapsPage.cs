// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Extensions;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public class MapsPage : BasePage
    {
        private ILocator Directions => _page.GetByRole(AriaRole.Button, new() { Name = "Directions" });

        private ILocator StartingPoint => _page.GetByRole(AriaRole.Textbox, new() { Name = "Choose starting point, or" });

        private ILocator AddDestinationPoint => _page.GetByRole(AriaRole.Button, new() { Name = "Add destination" });
        private ILocator DestinationPoint => _page.GetByRole(AriaRole.Textbox, new() { Name = "Choose destination, or click" });
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
        
        public async Task SetRouteLocationsAndSearchAsync(string startLocationName, string [] arrStopLocations = null!)
        {
            if (arrStopLocations.Length == 0)
                throw new ArgumentException("At least one stop locations are required for a route.");
            await SearchBox.FillAsync(arrStopLocations[0]);
            await SearchButton.ClickAsync();
            await Directions.ClickAsync();
            await StartingPoint.SetTextValueAndEnter(startLocationName);
            if(arrStopLocations.Length > 1)
            {
                for(int i = 1; i < arrStopLocations.Length; i++)
                {                    
                    await AddDestinationPoint.ClickAsync();
                    await DestinationPoint.SetTextValueAndEnter(arrStopLocations[i]);
                }
            }
        }

        public async Task SelectFirstResultAsync()
        {
            await FirstResult.WaitAndClickAsync();
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