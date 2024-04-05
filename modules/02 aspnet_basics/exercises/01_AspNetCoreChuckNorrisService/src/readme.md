# Übung AspNetCoreChuckNorrisService

In dieser Übung stürzen wir uns in unseren ersten Webservice mit ASP.NET Minimal APIs

Viel Spaß dabei! 🎉

> [!NOTE]Hinweis
> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Stellen Sie sicher das Sie den Endpunkt in der Vorlage mit einem HTTP-Tool Ihrer Wahl aufrufen können. Ändern Sie die Implementierung nun so ab, das ohne den direkten Zugriff auf den `HttpContext` gearbeitet wird.

## Aufgabe 2

Fügen Sie einen weiteren `Get`-Endpunkt hinzu, welcher einen `Joke` mit einer bestimmten Id zurück gibt. Nutzen Sie dazu die Route-Parameter-Möglichkeiten.
Erweitern Sie dazu das Interface `IJokeProvider` um die Methode `Task<Joke> GetJokeById(string id)` und implementieren Sie diese im `FileSystemJokeProvider`.

> [!NOTE]Hinweis
> Sie müssen auch die anderen Implementierungen des Interfaces anpassen. Sie können eine `NotImplementedException` werfen, um auszurdücken das diese noch nicht umgesetzt sind.

## Aufgabe 3

Bisher haben wir den `JokesProvider` immer direkt instanziiert. Das hat eine enge Koppelung zur `JokesProvider` Implementierung zur Folge. Nutzen Sie das Dependency Injection System, um eine Instanz eines JokesProviders über Dependency-Injection zu erhalten.

Überlegen Sie sich, welche Lifetime für Ihren `JokesProvider` angebracht ist.

## Aufgabe 4

Tauschen Sie die Verwendung des `FileSystemJokeProvider` durch den `ApiJokeProvider` aus. Falls noch nicht geschehen, implementieren Sie bitte die Methode `Task<Joke> GetJokeById(string id)` im `ApiJokeProvider` aus.
Testen Sie Ihre API mit dem Postman.

Sehen Sie den Vorteil der Dependency Injection? Haben sie eine andere Meinung zu DI?

### Zusatzaufgabe

Die Konfiguration der Services kann schnell unübersichtlich werden. Um den Überblick zu behalten, definieren Sie bitte eine Extension-Method namens `AddJokes` und registrieren sie dort Ihren Provider.
