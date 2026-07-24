using Microsoft.Extensions.DependencyInjection;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.Config;
using PlaywrightAutomationPoc.GoogleMaps.Pages;
using PlaywrightAutomationPoc.AutoFramework;
using Xunit.Abstractions;

namespace PlaywrightAutomationPoc.GoogleMaps.Tests;

/// <summary>
/// Test class for Google Maps related tests.
/// </summary>
public class MapsTests : IClassFixture<TestServiceFixture>, IAsyncLifetime
{
    private readonly ITestSetting _testSetting;
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly IMapsPage _mapsPage;
    private readonly string _testName;
    private readonly string _className;

    public MapsTests(TestServiceFixture fixture, ITestOutputHelper output)
    {
        _testSetting = fixture.ServiceProvider.GetRequiredService<ITestSetting>();
        _playwrightDriver = fixture.PlaywrightDriver;
        
        // ✅ No null-coalescing needed; Fixture initialized this before hitting this constructor
        _mapsPage = fixture.MapsPage; 
        // Use reflection on ITestOutputHelper to get the current test context safely
        var (_className, _testName) = XunitContextHelper.GetTestContext(output);
    }

    // ✅ Safely handle async pre-test operations here
    public async Task InitializeAsync()
    {
        // Use StartChunkAsync for tests sharing a ClassFixture context 
        // to prevent "Tracing has already been started" errors.
        await _playwrightDriver.Page.Context.Tracing.StartChunkAsync(new()
        {
            Title = $"{_className}.{_testName}",
        });
    }

    // ✅ Safely save the trace after the test finishes
    public async Task DisposeAsync()
    {
        var traceName = $"{_testName}_{Guid.NewGuid().ToString().Substring(0, 5)}.zip";
        await _playwrightDriver.Page.Context.Tracing.StopChunkAsync(new()
        {
            Path = $"Traces/{traceName}"
        });
    }

    [Theory(DisplayName = "SearchSpecificLocation")]
    [InlineData("Esker Educate Together National School")]
    public async Task SearchSpecificLocation(string input)
    {
        // ✅ Clean, readable test logic with no null checks
        await _mapsPage.NavigateAsync(_testSetting.BaseUrl);
        await _mapsPage.HandleCookiesAsync();
        await _mapsPage.SearchLocationAsync(input);

        var headline = await _mapsPage.GetHeadlineAsync(input);

        Assert.Contains(input, headline, StringComparison.OrdinalIgnoreCase);
    }
    
    [Theory]
    [InlineData("26 Tandy's Pl, Doddsborough", new string[] { "Esker Educate Together National School", "Verizon Connect Ireland" })]
    public async Task SetRouteDirections(string startLocation, string[] stopLocations)
    {
        if(stopLocations.Length < 1)
            throw new ArgumentException("At least 1 stop location must be provided.");
        
        // ✅ Clean, readable test logic
        await _mapsPage.NavigateAsync(_testSetting.BaseUrl);
        await _mapsPage.HandleCookiesAsync();
        await _mapsPage.SetRouteLocationsAndSearchAsync(startLocation, stopLocations);
        
        var routeTime = await _mapsPage.GetRouteOptionTimeAsync();
        
        Assert.Contains("min", routeTime, StringComparison.OrdinalIgnoreCase);
    }
}