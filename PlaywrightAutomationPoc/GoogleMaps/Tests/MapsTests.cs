// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Extensions.DependencyInjection;
using PlaywrightAutomationPoc.AutoFramework.Browser;
using PlaywrightAutomationPoc.AutoFramework.Reporter;
using PlaywrightAutomationPoc.Config;
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
    private readonly ITestSetting _testSetting;
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly IReportGenerator _reporter;
    private IMapsPage _mapsPage; // assigned per test or reused from fixture
    private IPage? _testPage; // when creating per-test contexts
    private readonly TestServiceFixture _fixture;
    private readonly string _testName;
    private readonly string _className;
    private StringBuilder _consoleLogs;
    private bool _traceStarted = false;

    public MapsTests(TestServiceFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _testSetting = fixture.ServiceProvider.GetRequiredService<ITestSetting>();
        _playwrightDriver = fixture.PlaywrightDriver;
        _reporter = fixture.Reporter;
        
        // Do not bind to a concrete page object here - create or reuse in InitializeAsync based on configuration
        // Use reflection on ITestOutputHelper to get the current test context safely
        (_className, _testName) = XunitContextHelper.GetTestContext(output);
        // Initialize report once per test run
        _reporter.CreateTest($"{_className}.{_testName}");
        _reporter.LogInfo($"Starting test {_className}.{_testName}.");
        // 4. Set up an in-memory buffer to capture browser logs during this test
        _consoleLogs = new StringBuilder();
        /*
        _mapsPage.Console += (_, msg) =>
        {
            _consoleLogs.AppendLine($"[{msg.Type.ToUpper()}] {msg.Text}");
        }; */
    }

    // ✅ Safely handle async pre-test operations here
    public async Task InitializeAsync()
    {
        // Decide whether to create an independent context per test or reuse the class-level context
        var reuseStrategy = _testSetting.ContextReuse?.ToLowerInvariant() ?? "perclass"; // "pertest" or "perclass"

        // Decide tracing policy. Values: "off", "on", "on-first-retry"
        var traceSetting = _testSetting.Trace?.ToLowerInvariant() ?? "off";
        var forceTrace = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PLAYWRIGHT_FORCE_TRACE"));
        var startTrace = traceSetting == "on" || (traceSetting == "on-first-retry" && forceTrace);

        if (reuseStrategy == "pertest")
        {
            // Create a fresh context and page for this test
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
            // Reuse the shared page from the fixture (faster)
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

    // ✅ Safely save the trace after the test finishes
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
                // Close the per-test context to free resources
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
            // ✅ Clean, readable test logic with no null checks
            await _mapsPage.NavigateAsync(_testSetting.BaseUrl);
            _reporter.LogInfo($"Navigated to {_testSetting.BaseUrl} successfully.");
            await _mapsPage.CookieConsent.HandleCookiesAsync();
            await _mapsPage.SearchLocationAsync(input);
            _reporter.LogInfo($"Search Location '{input}' executed successfully.");

            var headline = await _mapsPage.GetHeadlineAsync(input);

            Assert.Contains(input, headline, StringComparison.OrdinalIgnoreCase);
            _reporter.LogPass($"Test {_className}.{_testName} passed.");
        }
        catch (Exception ex)
        {
            _reporter.LogFail($"Test {_className}.{_testName} failed with exception: {ex.Message}");
            throw; // Rethrow to ensure the test fails
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
            
            // ✅ Clean, readable test logic
            await _mapsPage.NavigateAsync(_testSetting.BaseUrl);
            await _mapsPage.HandleCookiesAsync();
            await _mapsPage.SetRouteLocationsAndSearchAsync(startLocation, stopLocations);
            
            var routeTime = await _mapsPage.GetRouteOptionTimeAsync();
            
            Assert.Contains("min", routeTime, StringComparison.OrdinalIgnoreCase);
            _reporter.LogPass($"Test {_className}.{_testName} passed.");
        }
        catch (Exception ex)
        {
            _reporter.LogFail($"Test {_className}.{_testName} failed with exception: {ex.Message}");
            throw; // Rethrow to ensure the test fails
        }
    }
}