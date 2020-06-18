# Repository Pattern

> Software Pattern dienen dazu, Lösungswege für typische Probleme während der Entwicklung / Architektur einer Anwendung zu bieten.
> Sie sind nicht dazu gedacht, vorsorglich eingesetzt zu werden, da mit Vorteilen immer auch Nachteile wie z.B. erhöhte Komplexität einhergeht.
> Daher immer ein Auge darauf haben, ob der Einsatz eines Patterns die Aufgabe erleichtert oder nicht doch verkompliziert.

## Worum es geht

Die meisten Anwendungen müssen irgendwann Daten aus einer Datenquelle abfragen oder abspeichern. Setzt man z.B. auf direkte SQL Queries mit Dapper, entsteht so Code der stark an die Verwendung einer Datenbank und die 3rd Party Library gekoppelt sind. Besonders problematisch ist dies für die (Unit-)Testbarkeit der Applikation, da nun eine Verbindung zu einer Datenbank bestehen muss. Selbst wenn man die Möglichkeit hat, z.B. mit SQL-Lite oder einer lokalen DB dies zu bewerkstelligen, bleibt das Problem das die Erstellung von Tests deutlich komplizierter geworden ist, da meist pro Testlauf eine leere Datenbank und eine Grundbefüllung (Seeding) benötigt werden. Ein Nebeneffekt ist ebenfalls eine reduzierte Ausführungsgeschwindigkeit der Tests aufgrund der Voraussetzungen.

## Mögliche Abhilfe durch das Repository Pattern

Wir haben nun zwei Herausforderungen identifiziert:

- Enge Koppelung an die Datenzugriffstechnologie
- Verminderte / Erschwerte Testbarkeit mit Unittests

Hier setzt das Repository Pattern an. Es geht darum den **Datenzugriff** von der **Datenzugriffstechnologie** zu trennen. Hierzu wird ein Interface mit den benötigten Methoden für die **Create-Read-Update-Delete (CRUD)** definiert.

```csharp
public interface IJokeRepository
{
    Task<Joke> GetById(string id);
    Task<Joke> Add(Joke joke);
    Task<Joke> Update(Joke joke);
    Task Delete(string id);
}
```

Nun kann eine Implementierung die gewünschte Datenbanktechnologie implementieren, z.B. SQL.

```csharp
public class SqlJokeRepository : IJokeRepository
{
    public Task<Joke> Add(Joke joke)
    {
        // Zugriff über SQL
    }

    public Task Delete(string id)
    {
        // Zugriff über SQL
    }

    public Task<Joke> GetById(string id)
    {
        // Zugriff über SQL
    }

    public Task<Joke> Update(Joke joke)
    {
       // Zugriff über SQL
    }
}

```

Der Code welcher vorher direkt mit der DB interagiert hat, kann nun nur durch die bereitgestellte Schnittstelle zugreifen.

```csharp
[ApiController]
public class JokesController : ControllerBase
{
    private readonly IJokeRepository _jokeRepository;

    public JokesController(IJokeRepository jokeRepository)
    {
        _jokeRepository = jokeRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        Joke joke = await _jokeRepository.GetById(id);
        if (joke == null)
        {
            return NotFound();
        }

        return Ok(joke);
    }
}
```

Für Tests können wir nun z.B. ein In-Memory-Repository erfinden, oder wir nutzen ein Framework namens [Moq](https://github.com/Moq/moq4), welches uns erlaubt einzelne Methoden des Interfaces zu "implementieren" und feste Daten für unseren Testfall zurückzugeben.

Das Interface kann auch weitere Methoden für z.B. Filterungen definieren. Ein Repository sollte aber keine Fachlogik enthalten, sondern sich nur um die CRUD-Anforderungen kümmern.

## Kritik

In letzter Zeit gibt es einige kritische Stimmen an der Notwendigkeit des Patterns insbesondere im Umfeld von Entity Framework Core. Die grundlegende Argumentation ist, dass EF Core nie durch eine andere OR-Mapper Lösung ersetzt werden wird und daher eine unnötige Abstraktion mit den entsprechenden Einschränkungen verursacht.
Hier ist einer der Kritiken https://www.thereformedprogrammer.net/is-the-repository-pattern-useful-with-entity-framework-core/.
