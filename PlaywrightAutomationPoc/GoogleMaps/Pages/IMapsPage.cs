using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public interface IMapsPage
    {
        Task NavigateAsync(string baseUrl);
        Task SearchLocationAsync(string location);
        Task HandleCookiesAsync();
        Task SelectFirstResultAsync();
        Task<string> GetHeadlineAsync(string sDestination);
        Task SetRouteLocationsAndSearchAsync(string startLocationName, string [] arrStopLocations = null!);
        Task<string> GetRouteOptionTimeAsync();
    }
}