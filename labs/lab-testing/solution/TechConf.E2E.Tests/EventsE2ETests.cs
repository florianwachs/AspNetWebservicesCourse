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
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TechConf_AppHost>();

        _app = await appHost.BuildAsync();

        await _app.StartAsync();
        await _app.ResourceNotifications.WaitForResourceAsync(
            "web", KnownResourceStates.Running);

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });
    }

    [Fact]
    public async Task HomePage_ShowsTitle()
    {
        var page = await _browser!.NewPageAsync();
        var webUrl = _app!.GetEndpoint("web", "http").ToString();

        await page.GotoAsync(webUrl);

        await Expect(page.Locator("h1")).ToHaveTextAsync("TechConf Events");
    }

    [Fact]
    public async Task CanCreateEvent_AndSeeItInList()
    {
        var page = await _browser!.NewPageAsync();
        var webUrl = _app!.GetEndpoint("web", "http").ToString();

        await page.GotoAsync(webUrl);

        await page.FillAsync("[data-testid='event-title']", "Playwright E2E Conference");
        await page.FillAsync("[data-testid='event-description']", "End-to-end tested event");
        await page.FillAsync("[data-testid='event-date']", "2026-12-01");
        await page.FillAsync("[data-testid='event-location']", "Stuttgart");
        await page.ClickAsync("[data-testid='submit-event']");

        var eventsList = page.Locator("[data-testid='events-list']");
        await Expect(eventsList).ToContainTextAsync("Playwright E2E Conference");
        await Expect(eventsList).ToContainTextAsync("Stuttgart");
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
