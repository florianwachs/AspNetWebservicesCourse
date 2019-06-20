# Übung AspNetCoreChuckNorrisService Automapper

Wie in der Vorlesung angesprochen ist das zurückgeben des Datenmodells in vielerlei Hinsicht eine schlechte Idee. Viewmodells ermöglichen es, zugeschnittene Objekte für die API bereitzustellen. Automapper erleichtert uns das Mapping zwischen den verschiedenen Repräsentationen.

Viel Spaß dabei! :tada:

Verbesserungsvorschläge / Ideen sind immer willkommen!

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus einer vorangegangenen Übung enthält. Stellen Sie mit einem HttpClient Ihrer Wahl sicher, das die API auf Ihrem System funktioniert.



## Aufgabe 2

- [ ] Fügen Sie Automapper ihrem Projekt hinzu.
- [ ] Für den `GET` der Autoren soll folgendes JSON zurückgegeben werden:

```json
{
  "fullName": "Chuck Norris",
  "displayName": "Chuck Norris (28)",
  "age": 28
}
```

- [ ] Implementieren Sie eine geeignete DTO-Klasse und ein Automapper Profil.
- [ ] Testen Sie Ihren Endpunkt.

## Aufgabe 3

- [ ] Erweitern Sie `AuthorRepository` um eine Methode `Task<Author> Update(Author author)` (ergänzen Sie diese auch im Interface).
- [ ] Erweitern Sie `AuthorController` um einen Endpunkt zum Aktualisieren eines kompletten Authorobjektes.
- [ ] Rufen Sie den Endpunkt mittels eines HttpClients auf, und verändern Sie einen bestehenden Author.
- [ ] Denken Sie an die Vorlesung und überlegen Sie sich bitte welche Probleme bei diesem Ansatz auftreten können.

> Hinweis: Sie werden auf ein Problem beim Update stoßen!. Überlegen Sie kurz was das Problem ist

## Aufgabe 4

- [ ] Es soll am `AuthorController` für einen Aufrufer nur möglich sein, Vorname, Name und Alter zu ändern. Implementieren Sie dies bitte mittels Automapper.
- [ ] Testen Sie Ihren Endpunkt.

## Aufgabe 5

- [ ] `GET` am `AuthorController` soll zusätzlich die Witze eines Authors mit ausgeben. Dabei ist aber darauf zu achten, das nur der Witztext zurückgegeben wird.

```json
{
  "fullName": "Chuck Norris",
  "displayName": "Chuck Norris (28)",
  "jokes": ["Joke 1", "Joke 2"]
}
```

- [ ] Implementieren Sie, dass nur die ersten 5 Witze eines jeden Authors zurückgegeben werden.
