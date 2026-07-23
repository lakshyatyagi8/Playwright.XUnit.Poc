Playwright xUnit Automation Framework
This repository is a modular, scalable Test Automation Framework built with C# .NET, Playwright, xUnit, and Microsoft.Extensions.DependencyInjection. It implements the Page Object Model (POM) and uses a scoped Dependency Injection container to ensure safe parallel execution and test isolation.

🏗 Framework Architecture
AutoFramework: The core engine. Contains browser lifecycle management (BrowserProvider), wrappers (PlaywrightDriver), and configuration binding.  

Dependency Injection: Services and Page Objects are registered in ServiceCollectionExtensions.cs.  Test Isolation: A new DI scope is created per test via TestServiceFixture.cs. This guarantees each test gets its own isolated browser context.  Environment Config: Settings are driven by JSON files like Environment/Dev.json.

🛠 How to Add a New Page (Page Object Model)To keep the framework maintainable, we separate UI interactions from test logic. Follow these steps to add a new page:1. Create the InterfaceDefine the page actions in an interface. Create this in the relevant domain folder (e.g., DomainName/Pages/).C#public interface ILoginContextPage 
{
    Task LoginAsync(string username, string password);
    Task<bool> IsLoggedInAsync();
}
2. Create the ImplementationCreate the concrete class implementing the interface. Inject the IPage (or PlaywrightDriver) via the constructor.C#public class LoginContextPage : ILoginContextPage
{
    private readonly IPage _page;

    // Inject the page provided by the factory or driver
    public LoginContextPage(IPage page)
    {
        _page = page;
    }

    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync("#username", username);
        await _page.FillAsync("#password", password);
        await _page.ClickAsync("#login-button");
    }

    public async Task<bool> IsLoggedInAsync()
    {
        return await _page.Locator("#dashboard").IsVisibleAsync();
    }
}
3. Register the Page in Dependency InjectionOpen AutoFramework/Extensions/ServiceCollectionExtensions.cs and register your new page in the appropriate domain extension method using AddTransient.  C#public static IServiceCollection AddDomainPages(this IServiceCollection services)
{
    services.AddTransient<ILoginContextPage, LoginContextPage>();
    // Add other pages here
    return services;
}

🧪 How to Add New Test 
CodeTests should only contain test logic, assertions, and calls to injected Page Objects.1. Create the Test ClassCreate your test inside the relevant domain folder (e.g., DomainName/Tests/). Implement IClassFixture<TestServiceFixture> so xUnit knows to use the DI container.  
2. Inject DependenciesRequest the required Page Objects through the test class constructor.C#using Xunit;
using PlaywrightAutomationPoc.GoogleMaps.Tests; // Path to TestServiceFixture

public class LoginTests : IClassFixture<TestServiceFixture>
{
    private readonly ILoginContextPage _loginPage;

    public LoginTests(TestServiceFixture fixture)
    {
        // Resolve the page from the scoped DI container
        _loginPage = fixture.ServiceProvider.GetService<ILoginContextPage>();
    }

    [Fact]
    public async Task ValidUser_ShouldLoginSuccessfully()
    {
        // Act
        await _loginPage.LoginAsync("admin", "password123");
        var isLoggedIn = await _loginPage.IsLoggedInAsync();

        // Assert
        Assert.True(isLoggedIn);
    }
}

🚀 Best Practices for Optimal Framework Usage
1. Respect the DI ScopesDO NOT use AddSingleton for Pages, Drivers, or Browser Providers. This will cause parallel tests to fight over the same browser window.DO use AddScoped for Core Engine components (PlaywrightDriver, BrowserProvider) so they share the exact same browser context within a single test run.DO use AddTransient for Page Objects.
2. Never Use Static DriversAvoid creating static browser instances or drivers. Because xUnit runs tests in parallel by default, static variables will cause test contamination and flakiness. Always rely on the TestServiceFixture to hand you a safe, isolated instance.
3. Asynchronous TeardownIf you create custom services that require cleanup (like writing to a file or closing a database connection), ensure they implement IAsyncDisposable. The TestServiceFixture.cs automatically calls DisposeAsync() on the container, ensuring zero memory leaks.  
4. Configuration ManagementDo not hardcode URLs or timeouts. Update Environment/Dev.json and bind it to the TestSetting model so it can be injected safely anywhere in the framework.  