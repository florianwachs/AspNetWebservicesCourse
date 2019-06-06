# Übung AspNetCoreChuckNorrisService Automapper

Wie in der Vorlesung angesprochen ist das zurückgeben des Datenmodells in vielerlei Hinsicht eine schlechte Idee. Viewmodells ermöglichen es, zugeschnittene Objekte für die API bereitzustellen. Automapper erleichtert uns das Mapping zwischen den verschiedenen Repräsentationen.

Viel Spaß dabei! :tada:

Verbesserungsvorschläge / Ideen sind immer willkommen!

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

Vorgegeben ist eine Solution welche bereits das Grundgerüst aus der vorangegangenen CRUD Übung enthält. Stellen Sie mit einem HttpClient Ihrer Wahl sicher, das die API auf Ihrem System funktioniert.

## Aufgabe 2

- [ ] Fügen Sie Automapper ihrem Projekt hinzu
- 


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