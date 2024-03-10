# Übung AspNetCoreChuckNorrisService

Bisher haben sie schon einiges erreicht! Ihren eigenen Service samt Routing nur mit Middleware aufgebaut. Web Services benötigen aber häufig noch viele **Cross-Cutting-Features**, wie z.B. Authentifizierung / Autorisierung, Logging, Request-Filterung, usw. Dies alles mit den bisherigen Möglichkeiten die Sie kennen umzusetzen ist mühsam. Deshalb bietet Asp.Net eine Routing Middleware namens Minimal APIs. In der Vorlesung haben Sie bereits gelernt, wie diese definiert wird. In dieser Übung wollen wir das gelernte nun vertiefen. Statt wie bisher mittels direktem definieren über Middleware, bauen wir unsere API mit Hilfe von Controller auf.

Viel Spaß dabei! 🎉

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

In der Übung zur `Middleware` hatten Sie bereits erste Berührungspunkte mit der Minimal API durch `app.MapGet()`. In dieser Übung wollen wir aber statt direkt mit dem `HttpContext` zu arbeiten Objekte direkt zurückgeben statt selbst in den Response-Stream zu schreiben. In der Vorlage finden Sie noch die MapGet-Implementierung aus der Middleware Übung. Stellen Sie sicher das Sie den Endpunkt mit einem HTTP-Tool Ihrer Wahl aufrufen können. Ändern Sie die Implementierung nun so ab, das ohne den direkten Zugriff auf den `HttpContext` gearbeitet wird.

## Aufgabe 2

Fügen Sie einen weiteren `Get`-Endpunkt hinzu, welcher einen `Joke` mit einer bestimmten Id zurück gibt. Nutzen Sie dazu die Route-Parameter-Möglichkeiten.
Erweitern Sie dazu das Interface `IJokeProvider` um die Methode `Task<Joke> GetRandomJokeAsync()` und implementieren Sie diese im `FileSystemJokeProvider`.

> **Hinweis** Sie müssen auch die anderen Implementierungen des Interfaces anpassen. Sie können eine `NotImplementedException` werfen um auszurdücken das diese noch nicht umgesetzt sind.

## Aufgabe 3

Bisher haben wir den `JokesProvider` immer direkt instanziiert. Das hat eine enge Koppelung zur `JokesProvider` Implementierung zur Folge. Nutzen Sie das Dependency Injection System, um eine Instanz eines JokesProviders über Dependency-Injection zu erhalten.

Überlegen Sie sich, welche Lifetime für Ihren `JokesProvider` angebracht ist.

## Aufgabe 4

Tauschen Sie die Verwendung des `FileSystemJokeProvider` durch den `ApiJokeProvider` aus. Falls noch nicht geschehen, implementieren Sie bitte die Methode `Task<Joke> GetRandomJokeAsync()` im `ApiJokeProvider` aus.
Testen Sie Ihre API mit dem Postman.

Sehen Sie den Vorteil der Dependency Injection? Haben sie eine andere Meinung zu DI?

### Zusatzaufgabe

Die Konfiguration der Services kann schnell unübersichtlich werden. Um den Überblick zu behalten, definieren Sie bitte eine Extension-Method namens `AddJokes` und registrieren sie dort Ihren Provider.
