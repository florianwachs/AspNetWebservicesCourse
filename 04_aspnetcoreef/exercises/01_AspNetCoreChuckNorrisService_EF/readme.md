# Übung AspNetCoreChuckNorrisService Entity Framework Core

In den vorangegangenen Übungen haben wir immer mit Daten In-Memory gearbeitet. Für viele Anwendungsfälle ist dies auch das optimale Vorgehen, für ebenso viele allerdings nicht. In der Vorlesung haben wir Entity Framework Core behandelt, Microsofts modernen Ansatz für die Datenhaltung in relationalen Datenbanken. In dieser Übung werden wir das Vorgestellte praktisch anwenden.

Viel Spaß dabei! :tada:

Verbesserungsvorschläge / Ideen sind immer willkommen!

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus der vorangegangenen CRUD Übung enthält. Stellen Sie mit einem HttpClient Ihrer Wahl sicher, das die API auf Ihrem System funktioniert.

## Aufgabe 2

- [ ] Ergänzen Sie `Joke` und `JokeCategory` um notwendige Attribute, wir möchten nicht das die Ids von der Datenbank generiert werden
- [ ] Implementieren Sie einen `DbJokeContext` für die Modelklassen `Joke` und `JokeCategories`
- [ ] Registrieren Sie den Context im DI-System und verwenden Sie die In-Memory-Database des EF Core Frameworks
- [ ] Sorgen Sie dafür, dass Ihre Datenbank beim Starten mit Witzen und Kategorien gefüllt werden

> Hinweise: `InMemoryJokeRepository.Init`, [Async Startup Task](https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/)

## Aufgabe 3

- [ ] Implementieren Sie eine Klasse `EFJokeRepository` welche das Interface `IJokeRepository` implementiert und Ihren `JokeDbContext` verwendet.
- [ ] Registrieren Sie dieses `EFJokeRepository` in Ihrem DI-System
- [ ] Testen Sie die API mit einem HTTP-Client Ihrer Wahl

## Aufgabe 4

- [ ] Nutzen Sie statt der In-Memory-Datenbank den SQL-Provider mit der LocalDb (sollte ihr System keine LocalDb haben, versuchen Sie bitte SQLite)
- [ ] Erstellen Sie eine initiale Migration mit dem dotnet ef Tool
- [ ] Führen Sie die Migration gegen die Datenbank aus

> Hinweise: Googlen Sie den Connection-String für die LocalDb (oder Ihre Datenbank)