# Unit Testing mit ASP.NET Core Minimal API und XUnit

## Einführung

Unit Testing ist ein wesentlicher Bestandteil der Softwareentwicklung, der sicherstellt, dass unsere Anwendungen wie erwartet funktionieren. In diesem Leitfaden werden wir uns darauf konzentrieren, wie man Unit Tests für ASP.NET Core Minimal APIs mit XUnit erstellt.

## Voraussetzungen

- .NET SDK (Version 6.0 oder höher)
- Ein bestehendes ASP.NET Core Minimal API Projekt
- Grundlegendes Verständnis von C# und ASP.NET Core

## Testprojekt einrichten

Zuerst müssen wir ein Testprojekt erstellen und die notwendigen Abhängigkeiten hinzufügen.

```bash
dotnet new xunit -o MeinProjekt.Tests
cd MeinProjekt.Tests
dotnet add reference ../MeinProjekt/MeinProjekt.csproj
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

## WebApplicationFactory für Minimal API Tests

Die `WebApplicationFactory` ist eine Klasse, die uns hilft, unsere API für Tests zu starten und darauf zuzugreifen. Hier ist eine Beispielimplementierung:

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MeinProjekt.Tests.Infrastructure
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Hier können Dienste für Tests konfiguriert werden
                // z.B. Mock-Services registrieren
            });
        }
    }
}
```

## Unit Tests für API-Endpunkte schreiben

Hier ist ein Beispiel für einen Test einer Wetter-API:

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MeinProjekt.Tests.Infrastructure;
using Xunit;

namespace MeinProjekt.Tests
{
    public class WetterApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WetterApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_GibtWettervorhersagenZurück()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
            var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
            Assert.NotNull(forecasts);
            Assert.NotEmpty(forecasts);
        }
    }
}
```

## Tests für einzelne Handler (ohne HTTP)

Manchmal möchten wir Handler direkt testen, ohne HTTP-Anfragen zu senden:

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MeinProjekt.Tests
{
    public class WetterHandlerTests
    {
        [Fact]
        public void HoleWettervorhersage_GibtGültigeVorhersagenZurück()
        {
            // Arrange
            var wetterService = new WetterService();

            // Act
            var ergebnis = wetterService.HoleWettervorhersage();

            // Assert
            Assert.NotNull(ergebnis);
            Assert.Equal(5, ergebnis.Length);
        }
    }
}
```

## Tests mit simulierten Abhängigkeiten (Mocking)

Um Tests mit simulierten Diensten durchzuführen:

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MeinProjekt.Tests
{
    public class WetterApiMockTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public WetterApiMockTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_BenutztWetterService_UndGibtErgebnisseZurück()
        {
            // Arrange
            var mockWetterService = new Mock<IWetterService>();
            mockWetterService.Setup(s => s.HoleWettervorhersage())
                .Returns(new WeatherForecast[]
                {
                    new() { Date = DateTime.Now, TemperatureC = 20, Summary = "Mild" }
                });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(mockWetterService.Object);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
            var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
            Assert.Single(forecasts);
            Assert.Equal("Mild", forecasts[0].Summary);

            mockWetterService.Verify(s => s.HoleWettervorhersage(), Times.Once);
        }
    }
}
```

## Parametrisierte Tests

XUnit erlaubt uns, Tests mit verschiedenen Eingabeparametern auszuführen:

```csharp
using System.Threading.Tasks;
using Xunit;

namespace MeinProjekt.Tests
{
    public class TemperaturRechnerTests
    {
        [Theory]
        [InlineData(0, 32)]
        [InlineData(100, 212)]
        [InlineData(-40, -40)]
        public void CelsiusNachFahrenheit_BerechnetKorrektenWert(int celsius, int erwarteterFahrenheit)
        {
            // Arrange
            var rechner = new TemperaturRechner();

            // Act
            var ergebnis = rechner.CelsiusNachFahrenheit(celsius);

            // Assert
            Assert.Equal(erwarteterFahrenheit, ergebnis);
        }
    }
}
```

## Tests ausführen

Um alle Tests auszuführen:

```bash
dotnet test
```

Um einen bestimmten Test auszuführen:

```bash
dotnet test --filter "FullyQualifiedName=MeinProjekt.Tests.WetterApiTests.Get_GibtWettervorhersagenZurück"
```

## Tipps für effektives Testing

1. **Isolierte Tests**: Stellen Sie sicher, dass jeder Test unabhängig von anderen Tests ist.
2. **Arrange-Act-Assert**: Strukturieren Sie Tests nach diesem Muster.
3. **Beschreibende Namen**: Verwenden Sie aussagekräftige Testnamen.
4. **Mock-Abhängigkeiten**: Verwenden Sie Mocking für externe Dienste.
5. **Testdatenbanken**: Für Integrationstests mit Datenbanken separate Test-Datenbanken verwenden.

## Beispiel einer vollständigen Test-Suite

```csharp
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MeinProjekt.Tests.Infrastructure;
using Xunit;

namespace MeinProjekt.Tests
{
    public class BeispielTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BeispielTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetWeatherForecast_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task PostWeatherForecast_WithValidData_ReturnsCreated()
        {
            // Arrange
            var newForecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = 25,
                Summary = "Warm"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/weatherforecast", newForecast);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedForecast = await response.Content.ReadFromJsonAsync<WeatherForecast>();
            Assert.Equal(newForecast.Summary, returnedForecast.Summary);
        }
    }
}
```

## Umgang mit Authentifizierung / Autorisierung

Wir können uns hier für verschiedene Richtungen entscheiden. Die häufigsten sind:

- Ignorieren der Berechtigungen durch entfernen der Authentication
- Austausch der Autorisierung durch eine Dummy Auth

### Ignorieren der Berechtigungen

```csharp
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all existing authorization handlers
            services.AddSingleton<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, AllowAnonymousPolicyProvider>();
        });
        builder.UseEnvironment("Development");
    }
}

// Always allow authorization
public class AllowAnonymousAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var req in context.PendingRequirements.ToList())
            context.Succeed(req);
        return Task.CompletedTask;
    }
}

public class AllowAnonymousPolicyProvider : IAuthorizationPolicyProvider
{
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        Task.FromResult(new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build());

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) =>
        Task.FromResult<AuthorizationPolicy?>(new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build());

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
        Task.FromResult(new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build());
}
```

### Dummy Auth Handler

```csharp
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        });
    }
}

// Dummy Auth Handler
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin"), // Add roles/claims as needed
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

## Abschluss

Mit diesem Leitfaden solltet Ihr in der Lage sein, effektive Unit Tests für Eure ASP.NET Core Minimal API mit XUnit zu schreiben.
