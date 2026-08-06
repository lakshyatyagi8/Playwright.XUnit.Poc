// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.AutoFramework.Reporter;
using PlaywrightAutomationPoc.AutoFramework.Config;
using PlaywrightAutomationPoc.AutoFramework.Extensions;
using Xunit.Abstractions;
using PlaywrightAutomationPoc.GoogleMaps.Pages;
using System.Text;
using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.GoogleMaps.Tests;

/// <summary>
/// Test class for Google Maps related tests.
/// </summary>
public class MapsTests : IClassFixture<TestServiceFixture>, IAsyncLifetime
{
    private readonly PlaywrightSettings _playwrightSettings;
    private readonly ApplicationSettings _applicationSettings;
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly IReportGenerator _reporter;
    private IMapsPage _mapsPage; 
    private IPage? _testPage; 
    private readonly TestServiceFixture _fixture;
    private readonly string _testName;
    private readonly string _className;
    private StringBuilder _consoleLogs;
    private bool _traceStarted = false;
    private bool _testFailed = false;

    public MapsTests(
        TestServiceFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        // Retrieve settings from the test fixture to avoid xUnit fixture binding issues
        _playwrightSettings = fixture.PlaywrightSettings;
        _applicationSettings = fixture.ApplicationSettings;
        _playwrightDriver = fixture.PlaywrightDriver;
        _reporter = fixture.Reporter;
        _mapsPage = fixture.MapsPage;
        
        (_className, _testName) = XunitContextHelper.GetTestContext(output);
        _reporter.CreateTest($"{_className}.{_testName}");
        _reporter.LogInfo($"Starting test {_className}.{_testName}.");
        _consoleLogs = new StringBuilder();
    }

    public async Task InitializeAsync()
    {
        var reuseStrategy = "perclass"; 

        // 1. Initialize Page/Context based on strategy
        if (reuseStrategy == "pertest")
        {
            _testPage = await _playwrightDriver.CreateNewPageAsync();
            _mapsPage = new MapsPage(_testPage, _reporter);
        }
        else
        {
            _mapsPage = _fixture.MapsPage as MapsPage ?? throw new InvalidOperationException("MapsPage not found in fixture.");
        }

        // 2. Resolve the active context (Test-specific OR Global)
        var activeContext = _testPage?.Context ?? _playwrightDriver.Page.Context;
        // 3. Apply tracing to the resolved context
        var traceSetting = _playwrightSettings.Tracing?.ToLowerInvariant() ?? "off";
        
        if (traceSetting == "on" || (traceSetting == "on-first-retry") || traceSetting == "retain-on-failure")
        {
            if (reuseStrategy == "pertest" && _testPage != null)
            {
                var screenshotSetting = _playwrightSettings.Screenshot?.ToLowerInvariant() ?? "off";
                await activeContext.Tracing.StartAsync(new()
                {
                    Screenshots = screenshotSetting == "on" || screenshotSetting == "only-on-failure",
                    Snapshots = true,
                    Sources = true
                });
            }

            await activeContext.Tracing.StartChunkAsync(new() { Title = $"{_className}.{_testName}" });
            _traceStarted = true;
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            var activeContext = _testPage?.Context ?? _playwrightDriver.Page.Context;
            var traceSetting = _playwrightSettings.Tracing?.ToLowerInvariant() ?? "off";
            var shouldSaveTrace = traceSetting == "on" || (traceSetting == "retain-on-failure" && _testFailed);

            // 1. Handle Tracing uniformly
            if (_traceStarted && activeContext != null)
            {
                if (shouldSaveTrace)
                {
                    var traceName = $"Traces/{_testName}_{Guid.NewGuid().ToString("N")[..5]}.zip";
                    await activeContext.Tracing.StopChunkAsync(new() { Path = traceName });
                }
                else
                {
                    await activeContext.Tracing.StopChunkAsync(); // Discard memory buffer
                }
            }

            // 2. Close isolated context if using 'pertest'
            if (_testPage != null)
            {
                await _testPage.Context.CloseAsync();
                _testPage = null;
            }
        }
        catch (Exception ex)
        {
            _reporter.LogInfo($"Error while stopping trace or closing context: {ex.Message}");
        }
    }

    [Theory(DisplayName = "SearchSpecificLocation")]
    [InlineData("Esker Educate Together National School")]
    public async Task SearchSpecificLocation(string input)
    {
        try
        {
            await _mapsPage.NavigateAsync(_applicationSettings.BaseUrl);
            _reporter.LogInfo($"Navigated to {_applicationSettings.BaseUrl} successfully.");
            await _mapsPage.HandleCookiesAsync();
            await _mapsPage.SearchLocationAsync(input);
            _reporter.LogInfo($"Search Location '{input}' executed successfully.");

            var headline = await _mapsPage.GetHeadlineAsync(input);

            Assert.Contains(input, headline, StringComparison.OrdinalIgnoreCase);
            _reporter.LogPass($"Test {_className}.{_testName} passed.");
        }
        catch (Exception ex)
        {
            _testFailed = true;
            _reporter.LogFail($"Test {_className}.{_testName} failed with exception: {ex.Message}");
            throw;
        }
    }
    
    [Theory]
    [InlineData("26 Tandy's Pl, Doddsborough", new string[] { "Esker Educate Together National School", "Verizon Connect Ireland" }, "via L1030")]
    public async Task SetRouteDirections(string startLocation, string[] stopLocations, string expectedRouteName)
    {
        try
        {
            if(stopLocations.Length < 1)
                throw new ArgumentException("At least 1 stop location must be provided.");
            
            await _mapsPage.NavigateAsync(_applicationSettings.BaseUrl);
            await _mapsPage.HandleCookiesAsync();
            await _mapsPage.SetRouteLocationsAndSearchAsync(startLocation, stopLocations);
            
            var routeOptionFullString = await _mapsPage.GetRouteOptionInnerTextAsync(expectedRouteName);
            var routeTimeString = routeOptionFullString
                ?.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(s => s.Contains("min")) ?? string.Empty;
            Assert.Contains("min", routeTimeString, StringComparison.OrdinalIgnoreCase);
            _reporter.LogPass($"Test {_className}.{_testName} passed.");
        }
        catch (Exception ex)
        {
            _testFailed = true;
            _reporter.LogFail($"Test {_className}.{_testName} failed with exception: {ex.Message}");
            throw; 
        }
    }
}