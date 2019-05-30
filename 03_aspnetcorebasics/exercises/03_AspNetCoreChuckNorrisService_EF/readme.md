# Übung AspNetCoreChuckNorrisService Entity Framework Core

In den vorangegangenen Übungen haben wir immer mit Daten In-Memory gearbeitet. Für viele Anwendungsfälle ist dies auch das optimale Vorgehen, für ebenso viele allerdings nicht. In der Vorlesung haben wir Entity Framework Core behandelt, Microsofts modernen Ansatz für die Datenhaltung in relationalen Datenbanken. In dieser Übung werden wir das Vorgestellte praktisch anwenden. 

Viel Spaß dabei! :tada:

Verbesserungsvorschläge / Ideen sind immer willkommen!

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus der vorangegangenen CRUD Übung enthält. Stellen Sie mit einem HttpClient Ihrer Wahl sicher, das die API auf Ihrem System funktioniert.

## Aufgabe 2

- [ ] Erweitern Sie die Api um `Create (POST)`, `Read (GET)`, `Update (PUT)` und `Delete (DELETE)` Endpunkte.
- [ ] Ergänzen Sie die fehlende Implementierung in `JokesProvider`
- [ ] Testen Sie Ihre API mit Postman, Fiddler, VS Code ...

> Hinweise: **Sie müssen die Änderungen nicht wieder in die JSON-Datei zurückspeichern! Führen Sie die Änderungen bitte nur In-Memory aus.** Hint-Tags: `ControllerBase`, `Http[Verb]`

## Aufgabe 3

- [ ] Erweitern Sie die Api um einen partiellen Update Endpunkt (PATCH)
- [ ] Testen Sie das teilweise Aktualisieren mittels eines HTTP-Clients ihrer Wahl (z.B. Postman)

> Hinweise: [Microsoft Docs HTTP Patch](https://docs.microsoft.com/de-de/aspnet/core/web-api/jsonpatch?view=aspnetcore-2.2)

## Aufgabe 4

"Never trust the client" ist eine Regel die es stets zu befolgen gilt. Ein Ansatz gegen korrupte Daten ist ModelValidation. Nutzen Sie die aus der Vorlesung bekannten Data-Annotations.
Sorgen Sie bitte dafür das:

- [ ] ein Witz einen JokeText enthält
- [ ] der JokeText nicht länger als 500 Zeichen beträgt
- [ ] Versuchen Sie gültige / ungültige Witze an Ihre API zu schicken, nutzen Sie den Debugger um sich die Validierung im Controller anzusehen

## Aufgabe 5

Bei der Anwendung von Domain Driven Design wird besonders Wert darauf gelegt, das das Domänenmodell keinerlei Framework spezifische Abhängigkeiten hat. Selbst unsere Data-Annotations sollen vermieden werden.

- [ ] Implementieren Sie die die Validierungskriterien auf Aufgabe 3 mittels des Fluent-Validation-Frameworks aus der Vorlesung.
- [ ] Implementieren Sie einen Validator welcher falls Kategorien angegeben sind, sicherstellt das die Werte nur jeweils einmal vorkommen.

> Diskussion: Ihre Meinung ist gefragt. Welche Vor- / Nachteile haben die beiden Ansätze Ihrer Meinung nach? Welchen Ansatz würden Sie bevorzugen?

## Aufgabe 6

Sie haben nun die API direkt mittels Middleware implementiert und anschließend über die Controller-Abstraktion des MVC Frameworks

> Diskussion: Was sind Ihre Eindrücke zu den bisherigen Ansätzen. Können Sie sich Fälle vorstellen, in welchen Sie die direkte Middleware Implementierung bevorzugen?
