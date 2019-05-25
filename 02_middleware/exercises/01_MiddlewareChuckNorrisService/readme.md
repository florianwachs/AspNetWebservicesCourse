# Übung MiddlewareChuckNorrisService

In dieser Übung behandeln wir eines der Kernkonzepte von Asp.Net Core: **Middleware**.
Ausgangspunkt ist die Konsolenanwendung aus der ersten Übung. Diese werden wir nun in eine
erste Version des Webservices refaktorisieren.

Viel Spaß dabei!

> Nutzen Sie auch [Microsoft Docs](https://docs.microsoft.com).

## Aufgabe 1

In der Vorlesung haben Sie gelernt, das zum Ausführen einer Asp.Net Core Applikation ein (Web-) Host benötigt wird. Erweitern Sie bitte `Program.cs` um einen entsprechende Builder-Konfiguration und starten diesen in der `Main`-Methode.

> **Hinweise**: Stellen Sie sicher das sie alle benötigten NuGet-Pakete über den NuGet-Package-Manager installiert haben.

## Aufgabe 2

Während der Vorlesung haben wir behandelt, wie sie die Middleware-Pipeline sowohl durch NuGet-Pakete, als auch eigene Definitionen um Funktionalität anreichern können. Implementieren Sie bitte eine **Short-Circuit** Middleware, welche jeden Request mit einem zufälligen Witz als JSON beantwortet.

> **Hinweise**: `app.Run`, `JsonConvert`

## Aufgabe 3

Implementieren Sie bitte eine Middleware, welche die Antwort an den Client um 2s verzögert mittels `app.Use`. Der zufällige Witz aus **Aufgabe 2** soll weiterhin in der eigenständigen Middleware-Komponente ausgegeben werden.

### Zusatzaufgabe

Diese Middleware soll nur im `DEV`-Environment in die Pipeline eingehängt werden.

> **Hinweise**: `app.Use`, `app.Run`, `Environment`, `Stopwatch`, `Console`

## Aufgabe 4

Bisher reagiert unser Service auf jeden Request mit der gleichen Antwort. Wir wollen eine API implementieren, welche nur beim Aufruf von `api/jokes/random` mittels eines [GET]-Requests eine zufälligen Witz liefert. Nutzen Sie dazu die Routing-Komponente des MVC-Frameworks. In allen anderen Fällen soll dem Aufrufer eine Antwort mit dem Statuscode `404 Bad Request` zurückgegeben werden.

> **Hinweise**: `RouteBuilder`, `app.AddRouter`, `app.UseRouter`
