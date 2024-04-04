# Übung AspNetCoreChuckNorrisService mit Create / Read / Update / Delete

Bisher haben wir lediglich `GET`-Requests behandelt. Es wird Zeit, Create-Read-Update-Delete (CRUD) zu ermöglichen. In der Vorlesung haben Sie gelernt, welche Http-Verben für welchen Fall am geeignetsten sind. In dieser Übung wollen wir das Gelernte praktisch anwenden.

Viel Spaß dabei! 🎉

>[!NOTE]Hinweis
Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus der vorangegangenen Übung enthält. `Get`-Requests gegen den Endpunkt `api/jokes/random` sollten wie bisher auch von der API ausgeführt werden.

Stellen Sie bitte sicher das der aktuelle Stand einen `GET`-Request gegen `api/jokes/random` erfolgreich bearbeiten kann.

## Aufgabe 2

- [ ] Erweitern Sie die API um `Create (POST)`, `Read (GET)`, `Update (PUT)` und `Delete (DELETE)` Endpunkte.
- [ ] Ergänzen Sie die fehlende Implementierung in `JokesProvider`
- [ ] Testen Sie Ihre API mit Postman, Fiddler, VS Code ...

>[!NOTE]Hinweis
> Sie müssen die Änderungen nicht wieder in die JSON-Datei zurück speichern! Führen Sie die Änderungen bitte nur In-Memory aus.

## Aufgabe 3

"Never trust the client" ist eine Regel die es stets zu befolgen gilt. Ein Ansatz gegen korrupte Daten ist Validierung. Nutzen Sie die aus der Vorlesung bekannten `FluentValidation`-Libary.
Sorgen Sie bitte dafür das:

- [ ] ein Witz einen Value enthält
- [ ] der Value nicht länger als 500 Zeichen beträgt
- [ ] Versuchen Sie gültige / ungültige Witze an Ihre API zu schicken, nutzen Sie den Debugger um sich die Validierung anzusehen
- [ ] Implementieren Sie einen Validator welcher falls Kategorien angegeben sind, sicherstellt das die Werte nur jeweils einmal vorkommen.

>[!NOTE]Hinweis
> Stellen Sie sicher das Sie die Nuget-Pakete hinzugefügt haben

## Aufgabe 4

Die Rest-Guidelines definieren spezielle HTTP-Status-Codes. Nutzen Sie die `Results` um dem Aufrufer korrekte Statuscodes zurück zu geben.
Überlegen Sie sich für jeden Endpunkt welchen Status Sie zurückgeben würden.
Testen Sie Ihre API mit Postman, Fiddler, VS Code ...
