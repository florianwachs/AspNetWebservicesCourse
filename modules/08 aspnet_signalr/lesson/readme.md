# SignalR

## Worum geht es bei SignalR

- Realtime Kommunikation zwischen Clients und Servern
- „Scale Out“: Möglichkeit über mehrere Server hinweg Nachrichten zu verschicken
- Unterstützung von JS, .NET, Java und weiteren Client-Architekturen
- Ursprünglich erfunden von [David Fowler](https://github.com/davidfowl) und [Damian Edwards](https://github.com/DamianEdwards)

## Wie kann uns SignalR bei der Echzeitkommunikation unterstützen?

Ursprünglich war eine Zwei-Wege-Kommunikation in HTTP nicht vorgesehen. Die häufigste Lösung dennoch über Änderungen auf Serverseite informiert zu werden, ohne die Seite neu zu laden, ist `Polling`. Dabei wird in regelmäßigen Intervallen der Server angefragt, ob es Neuigkeiten für den Client gibt.

![Ohne SignalR müssen wir pollen](assets/signalr_polling.png)

Dieses Vorgehen ist sehr ineffizient. Über die Jahre gab es mehrere Erweiterungen wie Server Sent Events, um eine persistente Kommunikation zwischen Client und Server herzustellen. Die neueste Technologie sind Web-Sockets. Damit wird es Client und Server ermöglicht, ein bidirektionalen Kommunikationskanal herzustellen.

![WebSockets](assets/signalr_websockets.png)

Um die Verwendung von Web-Sockets für den Anwender transparent zu machen, wurde SignalR eingeführt.

SignalR bietet dem Entwickler eine API, um eine RPC-ähnliche Echtzeitkommunikation umzusetzen, ohne das sich dieser mit den unterstützten Technologien auseinandersetzen oder unterschiedliche Implementierungen bereitstellen zu müssen.

SignalR ist fester Bestandteil von `aspnetcore` und integriert sich nahtlos ins Endpoint-Routing System.

## Unterstützung von Web-Sockets

![Web Sockets Browser Support](assets/websocket_availability.png)

Web Sockets werden von allen modernen Browsern unterstützt.

## Web-Socket Fallbacks

Die Probleme beginnen jedoch, wenn Client oder Server keine Web-Sockets unterstützen, oder diese durch Firewalls geblockt werden.

- Websockets müssen vom Betriebssystem / Webserver / Firewall zugelassen und unterstützt werden
- Windows: Mind. Server 2008 R2 oder Windows 8

![Web Socket Fallbacks](assets/websocket_fallbacks.png)

| Technologie        |                                                                                                                                                                                                          |
| ------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Web-Socket         | Echte, bi-direktionale Verbindung zwischen Server und Client.                                                                                                                                            |
| Server Sent Events | In allen Browsern außer IE verfügbar.                                                                                                                                                                    |
| Forever Frame      | Ein verstecktes IFrame auf der Website das nie mit dem Laden fertig wird. Über das IFrame werden kontinuierlich script-Tags empfangen und ausgewertet. Nicht in der ASP.NET Core Variante verfügbar.     |
| Ajax Long Polling  | Fragt den Server in einem bestimmten Intervall ab, die Verbindung wird solange wie möglich aufrecht erhalten. Bei einem Timeout wird eine neue Verbindung aufgebaut. Nicht in der ASP.NET Core Variante. |

Für SignalR auf aspnetcore wurde der komplette Stack neu geschrieben und die Performance deutlich erhöht. Mehrere 10 tausend Verbindungen stellen für einen Server kein Problem dar.
SignalR wird auch in der neuesten Web-UI Technologie `Blazor` (server-side) eingesetzt, um UI Aktualisierungen ohne Postbacks zu realisieren.

## SignalR Hub

- High-Level Api über die Client(s) und Server miteinander kommunizieren können
- Als Protokolle werden ein JSON-Format und ein binäres MessagePack-Format unterstützt
- Client kann Methoden am Server aufrufen
- Server kann Methoden am Client aufrufen (Oha!)
- RPC-Style
- Als Clients werden aktuell .NET, JS, Java und ab 3.0 C++ unterstützt

## SignalR Clients

- C#: NuGet: Microsoft.AspNetCore.SignalR.Client
- TypeScript: npm: @aspnet/signalr

## Konfigurieren von SignalR

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services.AddSignalR();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        // ...

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatHub>("/chatHub");
        });
    }
}
```

## Hubs

Hubs sind die High-Level-API von SignalR. In einem Hub werden die Methoden definiert, welche von Clients aufgerufen werden können.
Zusätzlich sind Methoden in Hubs in der Lage ihrerseits Methoden auf dem Client auszulösen.
Die Verbindung zu Hubs wir vom Client-SDK hergestellt. Jede Verbindung zum Hub bekommt ihre eigene ConnectionId. Das kann auch passieren,
wenn das gleiche Browserfenster die Verbindung erneut zum Hub aufbaut.

```csharp
public class ChatHub : Hub<IChatHub>
{
    public async Task SendMessage(string user, string message)
    {
        // Aufruf nur beim Aufrufer des Hubs
        await Clients.Caller.ReceiveMessage(user, message);

        // Aufruf bei allen mit dem Hub verbundenen Clients
        await Clients.All.ReceiveMessage(user, message);

        // Aufruf bei allen Usern ausgenommen dem Aufrufer des Hubs
        await Clients.Others.ReceiveMessage(user, message);
    }
}

public interface IChatHub
{
    Task ReceiveMessage(string user, string message);
}
```

Nachrichten können auch außerhalb des Hubs an die Clients geschickt werden. Zu bedenken ist aber, dass es hier keinen `Caller` gibt.

```csharp
public class WeatherServices
{
    //                     👇 Ein Hub-Context kann über das DI System angefragt werden
    public WeatherServices(IHubContext<WeatherHub, IWeatherHub> hub)
    {
        _hub = hub;
    }

    private void UpdateWeather(object state)
    {
        // 👇 Es können Nachrichten an Alle oder Gruppen geschickt werden
        _hub.Clients.All.WeatherUpdated(forecast);
    }
}
```

## Clients

Clients müssen die Verbindung zu einem Hub herstellen. Für einige Sprachen gibt es bereits SDKs, welche die Nutzung von SignalR ermöglichen.

Für JavaScript kann das npm-Paket `@aspnet/signalr` verwendet werden.

```typescript
import * as signalR from "@aspnet/signalr";


const connection: signalR.HubConnection = new signalR.HubConnectionBuilder() // 👈 Der Builder kümmert sich um die Konfiguration
                                            .withUrl("/chatHub") // 👈 Per Convention ist die Url zu einem Hub [HubName]Hub
                                            .build(); // 👈 Hier wird noch keine Verbindung aufgebaut, nur konfiguriert


//               👇 Diese Methode wird vom Server in den Clients aufgerufen
connection.on("receiveMessage", (username: string, message: string) => {
  // ...
});

function send() {
    //          👇 Mittels send können Methoden auf Serverseite aufgerufen werden, auch mit komplexen Parametern
    connection.send("sendMessage", this.state.userName, this.state.text);
}


connection
  .start() // 👈 Der Verbindungsaufbau muss manuell ausgelöst werden
  .then(() => {/*connected*/})
  .catch((err) => /*...*/);
```

## Ressourcen

- [Beispiel Code der Vorlesung](src)
- [SignalR auf docs.microsoft.com](https://docs.microsoft.com/de-de/aspnet/core/signalr/introduction?view=aspnetcore-3.1)
