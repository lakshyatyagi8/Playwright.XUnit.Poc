using Microsoft.Extensions.DependencyInjection;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.Config;
using PlaywrightAutomationPoc.GoogleMaps.Pages;

namespace PlaywrightAutomationPoc.GoogleMaps.Tests;

/// <summary>
/// Test class for Google Maps related tests.
/// By implementing the IClassFixture<T> interface, you tell xUnit.net to create an instance of the specified class, TestServiceFixture, before any tests in MapsTests run.
public class MapsTests : IClassFixture<TestServiceFixture>
{
    private readonly ITestSetting _testSetting;
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly IMapsPage? _mapsPage;
    /// <summary>
    /// Initializes a new instance of the <see cref="MapsTests"/> class.
    public MapsTests(TestServiceFixture fixture)
    {
        _testSetting = fixture.Services.GetRequiredService<ITestSetting>();
        _playwrightDriver = fixture.PlaywrightDriver;
        _mapsPage = fixture.MapsPage;
        _playwrightDriver.Page.Context.Tracing.StartAsync(new()
        {
            Title = $"{WithTestNameAttribute.CurrentClassName}.{WithTestNameAttribute.CurrentTestName}",
            Screenshots = true,
            Snapshots = true,
            Sources = true 
        });
    }

    [Theory(DisplayName = "SearchSpecificLocation")]
    [InlineData("Esker Educate Together National School")]
    public async Task SearchSpecificLocation(string input)
    {
        
        if(_mapsPage == null)
            throw new InvalidOperationException("MapsPage is not initialized.");
        // PlaywrightDriver is initialized by the fixture
        await _mapsPage.NavigateAsync(_testSetting.BaseUrl);
        await _mapsPage.HandleCookiesAsync();
        await _mapsPage.SearchLocationAsync(input);

        var headline = await _mapsPage.GetHeadlineAsync(input);

        Assert.Contains(input, headline, StringComparison.OrdinalIgnoreCase);
    }
    
    [Theory]
    [InlineData("Esker Educate Together National School", new string[] { "Verizon Connect Ireland", "26 Tandy's Pl, Doddsborough" })]
    public async Task SetRouteDirections(string startLocation, string[] stopLocations)
    {
        if(_mapsPage == null)
            throw new InvalidOperationException("MapsPage is not initialized.");            
        if(stopLocations.Length < 1)
            throw new ArgumentException("At least 1 stop location must be provided.");
        
        await _mapsPage.NavigateAsync(_testSetting.BaseUrl);
        await _mapsPage.HandleCookiesAsync();
        await _mapsPage.SetRouteLocationsAndSearchAsync(startLocation, stopLocations);
        var routeTime = await _mapsPage.GetRouteOptionTimeAsync();
        Assert.Contains("min", routeTime, StringComparison.OrdinalIgnoreCase);
    }
}