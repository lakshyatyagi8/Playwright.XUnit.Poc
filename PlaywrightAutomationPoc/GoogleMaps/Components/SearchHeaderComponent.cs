// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public class SearchHeaderComponent
    {
        private readonly IPage _page;
        protected ILocator SearchBox => _page.Locator("input[name='q']");        
        protected ILocator SearchButton => _page.GetByRole(AriaRole.Button, new() { Name = "Search" });

        public SearchHeaderComponent(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }
        
        /// <summary>
        /// Searches for a location using the search box.
        /// </summary>
        /// <param name="locationName">Location name to search for</param>
        /// <returns></returns>
        public async Task SearchLocationAsync(string locationName)
        {
            await SearchBox.FillAsync(locationName);
            var responseTask = await _page.RunAndWaitForResponseAsync(async() => 
                await SearchButton.ClickAsync(),
                response => response.Url.Contains("/search?tbm=map") && response.Status == 200
            );            
            if(!responseTask.Ok)
            {
                throw new Exception($"Failed to get directions. Status: {responseTask.Status}, URL: {responseTask.Url}");
            }
        }
    }
}