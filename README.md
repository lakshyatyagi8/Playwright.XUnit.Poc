# Playwright xUnit Automation Framework

This repository contains a modular and scalable test automation framework built with C# .NET, Playwright, xUnit, and Microsoft.Extensions.DependencyInjection. It follows the Page Object Model (POM) and uses a scoped dependency injection container to keep browser state isolated across tests.

## Framework highlights

- Browser lifecycle and Playwright page management are handled in the AutoFramework layer.
- Shared page behavior is centralized in BasePage, which provides common actions such as navigation, cookie handling, and reusable locators.
- Extent Reports are integrated for rich HTML reporting and are written to Reports/test-report.html.
- Playwright tracing is enabled per test and saved as ZIP files under the Traces folder.
- Test isolation is enforced through TestServiceFixture and scoped services.

## Architecture overview

- AutoFramework: core engine for browser setup, Playwright driver management, DI registration, and reporting.
- GoogleMaps: feature-specific pages and test classes for the Google Maps scenario.
- Environment/Dev.json: environment-specific test settings bound through the TestSetting model.

## Adding a new page

The current implementation uses concrete page classes that inherit from BasePage rather than requiring a separate interface for each page.

1. Create a new page class in the relevant domain folder, such as GoogleMaps/Pages.
2. Inherit from BasePage and add page-specific locators and actions.
3. Register the page as a transient service in ServiceCollectionExtensions.
4. Resolve it from the DI container inside the test class or fixture.

Example:

```csharp
public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page)
    {
    }

    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync("#username", username);
        await _page.FillAsync("#password", password);
        await _page.ClickAsync("#login-button");
    }
}
```

Registration:

```csharp
services.AddTransient<LoginPage>();
```

## Adding a new test

1. Create a test class in the relevant domain folder.
2. Use TestServiceFixture so the test receives the configured DI container and Playwright driver.
3. Resolve the needed page objects from the fixture or container.
4. Keep assertions and workflow steps in the test body, while page-specific UI logic stays in the page object.

## Reporting and tracing

- Extent Reports are initialized once per test run and create a test node for each executed test.
- The reporter logs informational, pass, and fail events directly into the HTML report.
- Playwright tracing is started before each test and stopped afterward, producing trace artifacts in the Traces folder.

## Best practices

- Do not use AddSingleton for pages, drivers, or browser providers.
- Use AddScoped for core engine services that must share a browser context safely.
- Use AddTransient for page objects so each test gets a clean instance.
- Keep shared navigation and cookie logic in BasePage instead of duplicating it in each page object.
- Keep test data in configuration files rather than hardcoding values in tests.
