// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Extensions.Options;
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

    public MapsTests(
        TestServiceFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        // Retrieve settings from the test fixture to avoid xUnit fixture binding issues
        _playwrightSettings = fixture._playwrightSettings;
        _applicationSettings = fixture._applicationSettings;
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

        var traceSetting = _playwrightSettings.Tracing?.ToLowerInvariant() ?? "off";
        var forceTrace = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PLAYWRIGHT_FORCE_TRACE"));
        var startTrace = traceSetting == "on" || (traceSetting == "on-first-retry" && forceTrace);

        if (reuseStrategy == "pertest")
        {
            _testPage = await _playwrightDriver.CreateNewPageAsync();
            _mapsPage = new MapsPage(_testPage, _reporter);
            if (startTrace)
            {
                await _testPage.Context.Tracing.StartChunkAsync(new() { Title = $"{_className}.{_testName}" });
                _traceStarted = true;
            }
        }
        else if (reuseStrategy == "perclass")
        {
            _mapsPage = _fixture.MapsPage;
            if (startTrace)
            {
                await _playwrightDriver.Page.Context.Tracing.StartChunkAsync(new() { Title = $"{_className}.{_testName}" });
                _traceStarted = true;
            }
        }
        else
        {
            throw new InvalidOperationException($"Unknown context reuse strategy: {reuseStrategy}");
        }
    }

    public async Task DisposeAsync()
    {
        var traceName = $"{_testName}_{Guid.NewGuid().ToString().Substring(0, 5)}.zip";
        try
        {
            if (_testPage != null)
            {
                if (_traceStarted)
                {
                    await _testPage.Context.Tracing.StopChunkAsync(new() { Path = $"Traces/{traceName}" });
                }
                await _testPage.Context.CloseAsync();
                _testPage = null;
            }
            else
            {
                if (_traceStarted)
                {
                    await _playwrightDriver.Page.Context.Tracing.StopChunkAsync(new() { Path = $"Traces/{traceName}" });
                }
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
            _reporter.LogFail($"Test {_className}.{_testName} failed with exception: {ex.Message}");
            throw; 
        }
    }
    
    [Theory]
    [InlineData("26 Tandy's Pl, Doddsborough", new string[] { "Esker Educate Together National School", "Verizon Connect Ireland" })]
    public async Task SetRouteDirections(string startLocation, string[] stopLocations)
    {
        try
        {
            if(stopLocations.Length < 1)
                throw new ArgumentException("At least 1 stop location must be provided.");
            
            await _mapsPage.NavigateAsync(_applicationSettings.BaseUrl);
            await _mapsPage.HandleCookiesAsync();
            await _mapsPage.SetRouteLocationsAndSearchAsync(startLocation, stopLocations);
            
            var routeOptionFullString = await _mapsPage.GetRouteOptionTimeAsync("via");
            var routeTimeString = routeOptionFullString
                ?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(s => s.Contains("min")) ?? string.Empty;
            Assert.Contains("min", routeTimeString, StringComparison.OrdinalIgnoreCase);
            _reporter.LogPass($"Test {_className}.{_testName} passed.");
        }
        catch (Exception ex)
        {
            _reporter.LogFail($"Test {_className}.{_testName} failed with exception: {ex.Message}");
            throw; 
        }
    }
}