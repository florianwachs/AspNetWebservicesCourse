# Übung AspNetCoreChuckNorrisService

Bisher haben wir lediglich `GET`-Requests behandelt. Es wird Zeit, Create-Read-Update-Delete (CRUD) zu ermöglichen. In der Vorlesung haben Sie gelernt, welche Http-Verben für welchen Fall am geeignetsten sind. In dieser Übung wollen wir das Gelernte praktisch anwenden.

Viel Spaß dabei! :tada:

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus der vorangegangenen Übung enthält. `Get`-Request sollten wie bisher auch von der API ausgeführt werden.

Stellen Sie bitte sicher das der aktuelle Stand einen `GET`-Request erfolgreich bearbeiten kann.

## Aufgabe 2

- [x] Implementieren Sie

> Hinweise: `ControllerBase`, `HttpGet`

## Aufgabe 3

Bisher haben wir den `JokesProvider` immer direkt instanziert. Das hat eine enge Koppelung von `JokesController` zur `JokesProvider` Implementierung zur Folge. Nutzen Sie das Dependency Injection System um eine Instanz eines JokesProviders über Constructor-Injection im Controller zu erhalten.

Überlegen Sie sich, welche Lifetime für Ihren `JokesProvider` angebracht ist.

### Zusatzaufgabe

Die `ConfigureServices`-Methode kann schnell unübersichtlich werden. Um den Überblick zu behalten, definieren Sie bitte eine Extension-Method namens `AddJokes` und registrieren sie dort Ihren Provider.
