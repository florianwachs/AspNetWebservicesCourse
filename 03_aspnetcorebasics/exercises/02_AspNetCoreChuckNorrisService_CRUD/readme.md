# Übung AspNetCoreChuckNorrisService

Bisher haben wir lediglich `GET`-Requests behandelt. Es wird Zeit, Create-Read-Update-Delete (CRUD) zu ermöglichen. In der Vorlesung haben Sie gelernt, welche Http-Verben für welchen Fall am geeignetsten sind. In dieser Übung wollen wir das Gelernte praktisch anwenden.

Viel Spaß dabei! :tada:

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus der vorangegangenen Übung enthält. `Get`-Request sollten wie bisher auch von der API ausgeführt werden.

Stellen Sie bitte sicher das der aktuelle Stand einen `GET`-Request erfolgreich bearbeiten kann.

## Aufgabe 2

- [ ] Erweitern Sie die Api um `Create`, `Update` und `Delete` Endpunkte.
- [ ] Ergänzen Sie die fehlende Implementierung in `JokesProvider`
- [ ] Testen Sie Ihre API mit Postman, Fiddler, ...

> Hinweise: **Sie müssen die Änderungen nicht wieder in die JSON-Datei zurückspeichern! Führen Sie die Änderungen bitte nur In-Memory aus.** Hint-Tags: `ControllerBase`, `Http[Verb]`

## Aufgabe 3

"Never trust the client" ist eine Regel die es stets zu befolgen gilt. Ein Ansatz gegen korrupte Daten ist ModelValidation. Nutzen Sie die aus der Vorlesung bekannten Data-Annotations.
Sorgen Sie bitte dafür das:

- [ ] ein Witz einen JokeText enthält
- [ ] der JokeText nicht länger als 500 Zeichen beträgt.

## Aufgabe 4

Bei der Anwendung von Domain Driven Design wird besonders Wert darauf gelegt, das das Domänenmodell keinerlei Framework spezifische Abhängigkeiten hat. Selbst unsere Data-Annotations sollen vermieden werden.

- [ ] Implementieren Sie die die Validierungskriterien auf Aufgabe 3 mittels des Fluent-Validation-Frameworks aus der Vorlesung.
- Implementieren Sie einen Validator welcher falls Kategorien angegeben sind, sicherstellt das die Werte nur jeweils einmal vorkommen.

> Diskussion: Ihre Meinung ist gefragt. Welche Vor- / Nachteile haben die beiden Ansätze Ihrer Meinung nach? Welchen Ansatz würden Sie bevorzugen?

## Aufgabe 5

Sie haben nun die API direkt mittels Middleware implementiert und anschließend über die Controller-Abstraktion des MVC Frameworks

> Diskussion: Was sind Ihre Eindrücke zu den bisherigen Ansätzen. Können Sie sich Fälle vorstellen, in welchen Sie die direkte Middleware Implementierung bevorzugen?
