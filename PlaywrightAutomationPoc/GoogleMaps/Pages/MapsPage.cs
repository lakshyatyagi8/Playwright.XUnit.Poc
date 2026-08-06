// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;
using PlaywrightAutomationPoc.AutoFramework.Extensions;
using PlaywrightAutomationPoc.AutoFramework.Reporter;
using System.Text.RegularExpressions;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public class MapsPage : BasePage, IMapsPage
    {
        
        // Locators are defined as properties (Encapsulation)
        private SearchHeaderComponent SearchHeader { get; }
        private ILocator Directions => _page.GetByRole(AriaRole.Button, new() { Name = "Directions" });

        private ILocator StartingPoint => _page.GetByRole(AriaRole.Textbox, new() { Name = "Choose starting point, or" });

        private ILocator AddDestinationPoint => _page.GetByRole(AriaRole.Button, new() { Name = "Add destination" });
        private ILocator DestinationPoint => _page.GetByRole(AriaRole.Textbox, new() { Name = "Choose destination, or click" });
        
        private ILocator Headline(string sDestination) => _page.Locator($"div[aria-label*='{sDestination}']").First;        
                
        // 2. Constructor
        // Passes the IPage instance up to the BasePage
        public MapsPage(IPage page, IReportGenerator reporter) : base(page, reporter)
        {
            // Instantiate the components, passing the Playwright page down
            SearchHeader = new SearchHeaderComponent(page);
        }

        public new async Task NavigateAsync(string baseUrl)
        {
            await base.NavigateAsync(baseUrl);
        }

        public async Task HandleCookiesAsync()
        {
            await CookieConsent.HandleCookiesAsync();
        }

        public async Task SearchLocationAsync(string locationName)
        {
            await SearchHeader.SearchLocationAsync(locationName);
        }
        
        public async Task SetRouteLocationsAndSearchAsync(string startLocationName, string [] arrStopLocations = null!)
        {
            if (arrStopLocations == null || arrStopLocations.Length == 0)
                throw new ArgumentException("At least one stop locations are required for a route.");
            await SearchLocationAsync(arrStopLocations[0]);
            
            await Directions.ClickAsync();
            // 1. Set up the network listener task BEFORE setting the starting point to avoid race conditions
            var responseTask = await _page.RunAndWaitForResponseAsync(async() => 
                await StartingPoint.SetTextValueAndEnter(startLocationName),
                "**/maps/preview/directions*"
            );
            if(!responseTask.Ok)
            {
                throw new InvalidOperationException($"Failed to get directions. Status: {responseTask.Status}, URL: {responseTask.Url}");
            }
            await AddMultipleDestinationPoints(arrStopLocations);
            
        }
        public async Task AddMultipleDestinationPoints(string[] arrStopLocations)
        {
            if (arrStopLocations.Length <= 1)
            {
                return;
            }

            foreach (var stopLocation in arrStopLocations.Skip(1))
            {
                await AddDestinationPoint.ClickAsync();
                var responseTask = await _page.RunAndWaitForResponseAsync(async () =>
                    await DestinationPoint.SetTextValueAndEnter(stopLocation),
                    "**/preview/directions*"
                );

                if (!responseTask.Ok)
                {
                    throw new InvalidOperationException($"Failed to get directions for stop '{stopLocation}'. Status: {responseTask.Status}, URL: {responseTask.Url}");
                }
            }
        }

        public async Task<string> GetHeadlineAsync(string sDestination)
        {
            await Headline(sDestination).WaitForAsync();
            return await Headline(sDestination).InnerTextAsync();
        }
        
        public async Task<string> GetRouteOptionInnerTextAsync(string routeOptionPartialText)
        {
            // 1. Locate the route card by its ARIA role and containing text
            var routeCard = _page.GetByRole(AriaRole.Link)
                                .Filter(new LocatorFilterOptions { HasText = routeOptionPartialText });

            // 2. Locate the specific div containing the minute duration using a Regex pattern
            var minutesLocator = routeCard.Locator("div")
                                        .Filter(new LocatorFilterOptions { HasTextRegex = new Regex(@"\d+\s*min") })
                                        .First;

            return await minutesLocator.InnerTextAsync();
        }
    }
}