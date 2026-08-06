namespace PlaywrightAutomationPoc.GoogleMaps.Pages
{
    public interface IMapsPage
    {
        Task NavigateAsync(string baseUrl);
        Task HandleCookiesAsync();
        Task SearchLocationAsync(string locationName);
        Task SetRouteLocationsAndSearchAsync(string startLocationName, string[] arrStopLocations = null!);
        Task<string> GetHeadlineAsync(string sDestination);
        Task<string> GetRouteOptionInnerTextAsync(string routeOptionPartialText);
    }
}