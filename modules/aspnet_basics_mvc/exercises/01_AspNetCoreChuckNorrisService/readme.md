# Übung AspNetCoreChuckNorrisService

Bisher haben sie schon einiges erreicht! Ihren eigenen Service samt Routing nur mit Middleware aufgebaut. Web Services benötigen aber häufig noch viele **Cross-Cutting-Features**, wie z.B. Authentifizierung / Autorisierung, Logging, Request-Filterung, usw. Dies alles mit den bisherigen Möglichkeiten die Sie kennen umzusetzen ist mühsam. Deshalb bietet das Asp.Net Core Framework eine MVC-Komponente. In der Vorlesung haben Sie bereits gelernt, was ein `Controller` ist und wie er definiert wird. In dieser Übung wollen wir das gelernte nun vertiefen. Statt wie bisher mittels direktem definieren über Middleware, bauen wir unsere API mit Hilfe von Controller auf.

Viel Spaß dabei! 🎉

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

In der Vorlesung haben Sie gelernt, das die MVC-Komponente ebenfalls als Middleware implementiert ist.
Stellen Sie sicher, dass Sie die MVC-Middleware in Ihrer `Startup.cs` korrekt eingebunden haben.

> **Hinweise**: Stellen Sie sicher das sie alle benötigten NuGet-Pakete über den NuGet-Package-Manager installiert haben.

## Aufgabe 2

Erstellen Sie sich einen neuen Unterordner `Controllers`. Implementieren Sie dort einen `Controller` welcher beim Aufruf von `api/jokes/random` einen zufälligen Witz zurück liefert.

> Hinweise: `[ApiController]`, `ControllerBase`, `HttpGet`

## Aufgabe 3

Bisher haben wir den `JokesProvider` immer direkt instanziiert. Das hat eine enge Koppelung von `JokesController` zur `JokesProvider` Implementierung zur Folge. Nutzen Sie das Dependency Injection System um eine Instanz eines JokesProviders über Constructor-Injection im Controller zu erhalten.

Überlegen Sie sich, welche Lifetime für Ihren `JokesProvider` angebracht ist.

### Zusatzaufgabe

Die `ConfigureServices`-Methode kann schnell unübersichtlich werden. Um den Überblick zu behalten, definieren Sie bitte eine Extension-Method namens `AddJokes` und registrieren sie dort Ihren Provider.
