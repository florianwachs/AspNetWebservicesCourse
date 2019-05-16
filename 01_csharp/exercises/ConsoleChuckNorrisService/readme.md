# Übung ConsoleChuckNorrisService

In dieser Übung geht es darum, sich mit .NET, C# und Visual Studio vertraut zu machen.
Kommen Sie bei Fragen gerne auf mich zu!
Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

In dieser Aufgabe stellen wir sicher, dass Ihre Entwicklungsumgebung funktioniert.

- Öffnen Sie die Solution `ChuckNorrisService.sln` im Ordner `ConsoleChuckNorrisService` in Visual Studio.
- Builden Sie die Solution und stellen Sie sicher das keine Kompilerfehler im Output-Window angezeigt werden.

> Sollte Ihr Visual Studio kein Output-Window anzeigen, können Sie im Menü unter
> View->Output einblenden.

## Aufgabe 2

In dieser Aufgabe implementieren wir unsere erste C# Klasse.

- Im Ordner `Models` finden Sie unter anderem ein Interface `IJokeProvider`.
- Implementieren Sie in dem Ordner `Providers` eine Klasse `DummyJokeProvider`.
- `DummyJokeProvider` soll schlicht einen beliebigen Witz zurück geben.
- Rufen Sie ihre Implementierung in der `Programm.cs` auf.

### Erweiterung
- Wählen Sie eine geeignete Speicherstruktur um mehrere Witze zur Laufzeit hinterlegen zu können.
- Fügen Sie dieser Struktur mehrere Witze hinzu
- Geben Sie einen zufälligen Witz auf die Console aus.

> Hinweise:
> `Task.FromResult`, `Console.WriteLine`, `Random`

## Aufgabe 3

In dieser Aufgabe greifen wir auf das Dateisystem zu und nutzen ein NuGet-Paket um eine JSON-Datei einzulesen.

- Fügen Sie ihrem Projekt das NuGet-Paket `JSON.NET` in der aktuellen Version hinzu.
- Implementieren Sie eine Klasse `FileSystemJokeProvider` welche die Witze aus der Datei `Data\jokes.json` einliest und einen zufälligen (nutzen Sie `Random`) Witz ausgiebt.
- Rufen Sie ihre Implementierung in der `Programm.cs` auf.

> Hinweise:
> `JsonConvert.DeserializeObject`,`File.ReadAllTextAsync`, `File.ReadAllTextAsync`

# Aufgabe 4

In dieser Aufgabe rufen wir eine API mittels `HttpClient` auf.

- Implementieren Sie eine Klasse `ApiJokeProvider`, welche die API `https://api.chucknorris.io/jokes/random` aufruft und den zurückgelieferten Witz auf der Console ausgiebt.

> Hinweise:
> Rufen sie [https://api.chucknorris.io/jokes/random](https://api.chucknorris.io/jokes/random) im Browser / Postman auf. Implementieren Sie eine Klasse, welche die zurückgelieferten JSON-Daten abbildet. Nutzen sie [Microsoft Docs](https://docs.microsoft.com) Beispiele für die Verwendung von `HttpClient` zu finden.
> `HttpClient`, `GetAsync`, `Content.ReadAsAsync`

# Viel Erfolg
