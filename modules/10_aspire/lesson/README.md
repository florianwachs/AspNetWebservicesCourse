# .NET Aspire

## Was ist .NET Aspire?

.NET Aspire ist eine opinionierte, cloud-native Fullstack-Lösung für .NET 8 und höher. Es ist ein Application Framework, das darauf ausgelegt ist, die Entwicklung, Bereitstellung und den Betrieb verteilter Anwendungen zu vereinfachen.

.NET Aspire bietet:
- Ein Komponenten-Modell für gemeinsam genutzte Dienste
- Standard-Konfigurationen für Resilienz und Telemetrie
- Eine lokale Entwicklungsumgebung mit Dashboard
- Integration mit Container-Orchestrierungssystemen

## Ziele und Vorteile

### Ziele von .NET Aspire
- **Vereinfachte Cloud-native Entwicklung**: Reduzierung der Komplexität bei der Erstellung verteilter Anwendungen
- **Standardisierte Best Practices**: Einheitliche Muster für Resilienz, Telemetrie und Service-Discovery
- **Nahtlose Zusammenarbeit**: Verbesserung des Entwicklungserlebnisses für Teams, die an verteilten Systemen arbeiten
- **Produktionsreife**: Unterstützung für den gesamten Lebenszyklus von der Entwicklung bis zur Produktion

### Vorteile
- **Konsistentes Programmiermodell**: Einheitliche Ansätze für häufige Aufgaben wie Service-Registrierung und -Discovery
- **Bessere Beobachtbarkeit**: Integrierte Telemetrie mit OpenTelemetry für Logs, Metriken und Traces
- **Verbesserte Resilienz**: Eingebaute Patterns wie Circuit Breaker, Retry-Logik und Timeout-Handling
- **Lokales Entwicklungsdashboard**: Visualisierung und Verwaltung von Services während der Entwicklung
- **Einfache Skalierung**: Unterstützung für Container-Orchestrierung und Cloud-Deployment

## Einstieg mit .NET Aspire Starter Templates

Um mit .NET Aspire zu beginnen, benötigen Ihr zunächst das neueste .NET SDK (mindestens .NET 8) und die Aspire-Vorlagen.

### Installation von .NET Aspire

Um .NET Aspire zu nutzen, benötigen Ihr:

1. Das neueste .NET SDK (mindestens .NET 8)
2. Die neuesten Visual Studio / VS Code Updates

Mit dem aktuellen .NET SDK sind die Aspire-Projektvorlagen bereits enthalten und müssen nicht mehr separat als Workload installiert werden.

Überprüfen Ihr Ihre Installation:

```bash
dotnet --list-sdks
```

### Erstellen einer neuen Aspire-Anwendung

#### Minimalbeispiel mit Standardservices
```bash
dotnet new aspire-starter -o MeineAspireApp
```

#### Vollständigeres Beispiel mit Web-Frontend
Beispiele für Aspire können hier gefunden werden: https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript 

### Projektstruktur

Nach dem Erstellen eines Projekts mit dem `aspire-starter` Template erhalten Ihr folgende Struktur:

```
MeineAspireApp/
├── MeineAspireApp.sln
├── MeineAspireApp.AppHost/
│   └── Program.cs              # Konfiguration der Services und Ressourcen
├── MeineAspireApp.ServiceDefaults/
│   └── Extensions.cs           # Gemeinsame Service-Konfigurationen
└── MeineAspireApp.ApiService/
    └── Program.cs              # API-Beispielanwendung
```

### Ausführen der Anwendung

```bash
cd MeineAspireApp
dotnet run --project MeineAspireApp.AppHost
```

Nach dem Start öffnet sich automatisch das Aspire Dashboard im Browser (standardmäßig unter http://localhost:18888), über das Ihr alle Services, Logs und Telemetriedaten einsehen können.

### Hinzufügen weiterer Services

Ihr können Ihrem Aspire-Projekt weitere Services hinzufügen:

```bash
# Hinzufügen eines Web API-Projekts
dotnet new webapi -o MeineAspireApp.NeueApi
dotnet sln add MeineAspireApp.NeueApi

# Hinzufügen einer Blazor-Anwendung
dotnet new blazor -o MeineAspireApp.WebApp
dotnet sln add MeineAspireApp.WebApp
```

Anschließend müssen Ihr die neuen Services im AppHost (Program.cs) registrieren.

## Nächste Schritte

- Erkunden Ihr die verschiedenen Aspire-Komponenten in NuGet
- Experimentieren Ihr mit der Konfiguration von Resilienz-Mustern
- Integrieren Ihr Datenbank-Services wie SQL Server oder Redis
- Testen Ihr die Telemetrie-Funktionen mit dem Dashboard

## Kernkonzepte in .NET Aspire

### Das AppHost-Projekt

Das AppHost-Projekt ist das zentrale Steuerungselement einer .NET Aspire-Anwendung. Es dient als Orchestrator und Konfigurationszentrale für alle Services und Ressourcen.

#### Hauptaufgaben des AppHost:
- **Orchestrierung**: Definiert, welche Services und Ressourcen (z.B. Datenbanken) Teil der Anwendung sind
- **Konfiguration**: Legt fest, wie diese Services miteinander kommunizieren
- **Ressourcenverwaltung**: Verwaltet Container-Ressourcen und deren Lebenszyklus
- **Dashboarding**: Ermöglicht die Beobachtung aller Services über das integrierte Dashboard

#### Beispiel Program.cs im AppHost:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Registrierung eines API-Services
var apiService = builder.AddProject<Projects.MeineAspireApp_ApiService>("apiservice");

// Registrierung einer Datenbank
var db = builder.AddSqlServer("sql-server")
                .AddDatabase("MeineDb");

// Verbindung der API mit der Datenbank
apiService.WithReference(db);

// Registrierung eines Frontend-Projekts, das auf die API zugreift
builder.AddProject<Projects.MeineAspireApp_WebApp>("webapp")
       .WithReference(apiService);

return builder.Build().RunAsync();
```

### Das ServiceDefaults-Projekt

Das ServiceDefaults-Projekt enthält gemeinsame Konfigurationen, die auf alle Services der Anwendung angewendet werden. Es fördert die Konsistenz und reduziert Code-Duplizierung.

#### Inhalte des ServiceDefaults:
- **Resilienzkonfiguration**: Gemeinsame Retry-Policies, Timeouts und Circuit-Breaker
- **Telemetrie-Setup**: Konfiguration von OpenTelemetry für Logs, Metriken und Traces
- **Health Checks**: Standardisierte Gesundheitsprüfungen für Services
- **Service Discovery**: Registrierung und Entdeckung von Services

#### Beispiel Extensions.cs im ServiceDefaults:

```csharp
public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
{
    // Hinzufügen von Standard-Health-Checks
    builder.AddDefaultHealthChecks();
    
    // Konfiguration von OpenTelemetry
    builder.AddObservability();
    
    // Konfiguration von Service Discovery
    builder.Services.AddServiceDiscovery();
    
    // Konfiguration von Resilienz-Patterns
    builder.Services.ConfigureHttpClientDefaults(http =>
    {
        // Automatische Erkennung von Services
        http.UseServiceDiscovery();
        
        // Standardmäßig Retries konfigurieren
        http.AddStandardResilienceHandler();
    });
    
    return builder;
}
```

### Service Discovery in .NET Aspire

Service Discovery ermöglicht es Services, andere Services im Netzwerk zu finden und mit ihnen zu kommunizieren, ohne deren genaue Adressen zu kennen.

#### Wie Service Discovery funktioniert:
1. Jeder Service wird beim Start automatisch registriert
2. Der AppHost verwaltet ein Verzeichnis aller verfügbaren Services
3. Services können andere Dienste über ihren Namen finden
4. Die tatsächlichen Netzwerkadressen werden automatisch aufgelöst

#### Beispiel für Service Discovery mit Minimal API:

```csharp
// Program.cs in einem API-Service
var builder = WebApplication.CreateBuilder(args);

// ServiceDefaults hinzufügen (beinhaltet Service Discovery)
builder.AddServiceDefaults();

// Benannten HTTP-Client für Service Discovery registrieren
builder.Services.AddHttpClient("WeatherClient", client =>
{
    // Service wird automatisch durch seinen Namen aufgelöst
    client.BaseAddress = new Uri("http://weather-service");
})
.AddServiceDiscovery(); // Aktiviert Service Discovery für diesen Client

// Optional: Starken Typen für die Wetterdaten definieren
builder.Services.AddSingleton<WeatherService>();

var app = builder.Build();

// Minimal API Endpunkt definieren
app.MapGet("/wetter", async (WeatherService weatherService) => 
{
    // Abruf der Daten vom anderen Service über Service Discovery
    var forecast = await weatherService.GetForecastAsync();
    return forecast;
});

app.Run();
```

#### Beispiel für einen Service, der über Service Discovery aufgerufen wird:

```csharp
public class WeatherService
{
    private readonly HttpClient _weatherClient;
    
    public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
    {
        // Client abrufen, der ServiceDiscovery verwendet
        _weatherClient = httpClientFactory.CreateClient("WeatherClient");
        logger.LogInformation("WeatherService initialisiert mit Service Discovery");
    }
    
    public async Task<IEnumerable<WeatherData>> GetForecastAsync()
    {
        // Der Service wird automatisch durch seinen Namen aufgelöst
        // Kein Hardcoding von URLs oder Ports nötig
        return await _weatherClient.GetFromJsonAsync<WeatherData[]>("/weatherforecast") 
            ?? Array.Empty<WeatherData>();
    }
}
```