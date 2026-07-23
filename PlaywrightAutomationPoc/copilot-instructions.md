Copilot SDET Guidelines: C# & Playwright Best Practices

This document serves as an architectural standard and Copilot prompt-engineering reference for building robust, scalable, and high-performance test automation frameworks using C# and Playwright .NET, anchored by SOLID principles.

1. Core Architectural & SOLID Principles

Use C# 13 syntax and .NET 8 patterns.
Keep PageObjects thin: no assertions inside page classes.
Do not modify generated files or `bin/obj` outputs.

Test automation code is production-quality code. Apply SOLID principles to your Page Object Models, API wrappers, and test setups:

Single Responsibility Principle (SRP):

Rule: A Page Object should only manage element interactions and state representation for a single page or component. Never place test assertions inside Page Objects.

Application: Keep assertions strictly within the test classes ([Test] or [Fact] methods). Page methods should return state, values, or element handles.

Open/Closed Principle (OCP):

Rule: Base classes (like BasePage) should be open for extension (inheritance) but closed for modification.

Application: Mark base page classes as abstract to prevent direct instantiation and enforce a consistent dependency graph across all child pages.

Liskov Substitution Principle (LSP):

Rule: Subclasses must be substitutable for their base classes without breaking the test runner or page flows.

Interface Segregation Principle (ISP):

Rule: Prefer lean, specific interfaces (e.g., specific driver or logging interfaces) over bloated utility contracts.

Dependency Inversion Principle (DIP):

Rule: Depend upon abstractions, not hardcoded concrete instances.

Application: Use Microsoft.Extensions.DependencyInjection to manage service lifetimes (Singleton, Scoped, Transient) for configuration options, loggers, API clients, and page objects. Inject IPage explicitly into page constructors rather than relying on static context variables.

2. Playwright .NET Locating & Execution Best Practices

Prioritize User-Centric Locators:

Always prefer semantic locators (GetByRole, GetByText, GetByLabel, GetByPlaceholder) over fragile structural CSS or XPath selectors. They map directly to accessibility trees and ensure resilience against UI redesigns.

Leverage Lazy Evaluation (ILocator vs IElementHandle):

Always use ILocator. Locators are lazy-evaluated and handle auto-retries automatically on every interaction. Avoid IElementHandle, which evaluates immediately and points to a static, fixed DOM node susceptible to stale element errors.

Shadow DOM Piercing:

Rely on Playwright's native locators, which automatically pierce Shadow DOM boundaries without requiring manual explicit combinators like >>>.

Efficient Dynamic Web Tables:

Avoid looping through thousands of DOM rows manually. Use filtered locators to narrow down scopes cleanly:

page.GetByRole(AriaRole.Row).Filter(new() { HasText = "TargetValue" })


Never Forget await on Async Calls:

Failing to await asynchronous calls (like Page.GotoAsync()) will cause execution to stream immediately to the next line, triggering race conditions and flaky assertions. Avoid async void entirely in helper methods; always return async Task to ensure exceptions are caught properly.

3. Asynchrony, Concurrency, and Resilience

Concurrent Independent Tasks:

When waiting for multiple independent operations (like concurrent API setups), use Task.WhenAll(task1, task2) instead of blocking synchronous thread waits like Task.WaitAll.

Resilience & Transient Fault Handling:

Use the Polly resilience library for handling transient network faults or flaky backend integrations with exponential backoff.

Parallel Test Isolation:

When running tests in parallel via NUnit ([Parallelizable]) or xUnit collections, always instantiate a fresh IBrowserContext per test method using fixtures or setup hooks to prevent cross-test contamination.

4. Data Management & Serialization

C# Records for API Payloads:

Use C# record types instead of standard classes for data transfer objects (DTOs) and API request/response payloads to gain built-in value-based equality and immutability out of the box.

Safe Data Extraction:

When using EvaluateAsync<T>() to extract data from the browser context, ensure data structures are fully serializable across the browser-to-.NET boundary to prevent runtime serialization exceptions.

Configuration & Secrets:

Never hardcode credentials or environment URLs. Leverage .NET configuration providers with Environment Variables or the .NET Secret Manager.

5. CI/CD Optimization, Tracing, and Artifacts

Bypass UI Login via Storage State:

Capture and save authentication cookies and local storage using Playwright's StorageState feature. Inject this state directly into browser contexts to bypass repetitive UI login sequences and drastically speed up CI execution times.

Optimized CI Artifact Retention:

To prevent your CI storage from overflowing with gigabytes of passing test videos and traces, configure retention policies conditionally:

// Example concept: Retain traces/videos only on failure or first retry
Video = RecordVideoMode.OnFirstRetry;


Centralized Teardown Hooks:

Place screenshot capture, trace dumping, and video logging inside centralized lifecycle teardown hooks ([TearDown] in NUnit or IAsyncLifetime in xUnit) rather than scattering boilerplate across every test method.