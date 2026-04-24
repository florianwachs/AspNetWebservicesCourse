using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Playwright;
using Xunit;

namespace TechConf.E2E.Tests;

public class EventsE2ETests : IAsyncLifetime
{
    private DistributedApplication? _app;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        // TODO: Create and start the AppHost-driven Aspire test app.
        // Hint: var appHost = await DistributedApplicationTestingBuilder
        //           .CreateAsync<Projects.TechConf_AppHost>();
        //       _app = await appHost.BuildAsync();

        // TODO: Wait for the "web" resource to be running.
        // This keeps the browser test on the same Aspire harness as the AppHost tests.
        // Hint: await _app.StartAsync();
        //       await _app.ResourceNotifications.WaitForResourceAsync(
        //           "web", KnownResourceStates.Running);

        // TODO: Launch Playwright and a Chromium browser
        // Hint: _playwright = await Playwright.CreateAsync();
        //       _browser = await _playwright.Chromium.LaunchAsync(
        //           new() { Headless = true });
    }

    [Fact]
    public async Task HomePage_ShowsTitle()
    {
        // TODO: Create a new page and navigate to the web app URL from the Aspire app.
        // Hint: var webUrl = _app!.GetEndpoint("web", "http").ToString();
        //       var page = await _browser!.NewPageAsync();
        //       await page.GotoAsync(webUrl);

        // TODO: Assert the page has an h1 with text "TechConf Events"
        // Hint: await Expect(page.Locator("h1")).ToHaveTextAsync("TechConf Events");
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CanCreateEvent_AndSeeItInList()
    {
        // TODO: Navigate to the web app through the Aspire-managed endpoint.

        // TODO: Fill in the event form using data-testid selectors:
        //   - [data-testid='event-title']
        //   - [data-testid='event-description']
        //   - [data-testid='event-date']
        //   - [data-testid='event-location']
        // Hint: await page.FillAsync("[data-testid='event-title']", "My Event");

        // TODO: Click the submit button [data-testid='submit-event']

        // TODO: Assert the event appears in [data-testid='events-list']
        // Hint: var eventsList = page.Locator("[data-testid='events-list']");
        //       await Expect(eventsList).ToContainTextAsync("My Event");
        throw new NotImplementedException();
    }

    private static ILocatorAssertions Expect(ILocator locator) =>
        Assertions.Expect(locator);

    public async Task DisposeAsync()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
        if (_app is not null) await _app.DisposeAsync();
    }
}
