# Entity Framework Core

Im Folgenden wollen wir uns die Grundlagen des Entity Frameworks ansehen. EF ist ein komplexes Framework, daher können hier nicht alle Funktionen behandelt werden. Ich empfehle die großartige Dokumentation auf https://docs.microsoft.com.

## Überblick

- Interaktion mit relationalen Datenbanken über ein Objektmodell das direkt die Business-Objekte der Anwendung abbildet
- Object Relational Mapper
- Favorisierte API für Datenzugriff in .NET
- Kümmert sich um den Aufbau und die Ausführung von SQL-Statements
- Umwandlung von Abfrageergebnissen in Business-Objekte
- Änderungsverfolgung an Business-Objekten
- LINQ ist Grundbestandteil der Architektur, nicht nachträglich hinzugefügt
- Reduzierte Abhängigkeiten zum physikalischen Datenbankschema und zu relationalen Datenbanken selbst (nur EF Core)
- Hohe Testbarkeit: die Business-Objekte können POCOs sein und enthalten damit keine Datenbankzugriffslogik und auch keine harten Abhängigkeiten zum EF
- Es werden keine Collections aus Zeilen und Spalten bearbeitet, sondern Collections aus typisierten Objekten, sog. Entitäten
- Entity Framework kommt in 2 unterschiedlichen Ansätzen
  - model-first (nur Full-Framework, EF Core kann aber ein Modell aus der DB generieren)
  - code-first

## Vorteile von OR-Mappern

- Bekanntes Umfeld in der objektorientierten Entwicklung
  - Kenntnisse von SQL, Datenbankschemas nicht zwingend notwendig
  - Höhere Entwicklungsgeschwindigkeit da die SQL-Tabellen zu einem späteren Zeitpunkt generiert werden können (In-Memory-Database)
- Automatische Änderungserkennung an Objekten und Generierung von SQL für das Update
- Abstraktion von der zugrundeliegenden Datenbank-Technologie
  - MS SQL
  - Postgres
  - SQLite
  - MySQL

## Nachteile von OR-Mappern

- Queries können inperformant werden (n+1 Problem)
- Insgesamt ist die Performance relativ gesehen zur direkten Implementierung langsamer (high-traffic)

## EF Core Building Blocks

Der Einsatz von EF Core im Projekt kann in folgende Bereiche aufgeteilt werden:

- DbContext und Entitäten modellieren
- DbContext im Dependency Injection System konfigurieren und registrieren
  - Datenprovider auswählen => MS SQLServer, PostgresSQL, Sqlite…
- Migrationen für das Datenmodell generieren
- Migrationen auf die Datenbank anwenden

### DbContext und Entitäten modellieren

Entitäten mit ihren Eigenschaften und Relationen werden bei EF Core als POCOs (Plain Old C# Objects) modelliert, d.h. sie müssen nicht von einer Basisklasse abgeleitet worden sein oder ein spezielles Interface implementieren. Über Attribute kann zusätzliche Funktionalität wie Validierung oder datenbankspezifische Einstellungen wie Spaltenname konfiguriert werden. Will man auch keine Attribute haben (häufig bei Domain Driven Design), so kann diese Konfiguration über spezielle Klassen gelöst werden.

Hier die `Author`-Entität als Beispiel.

```csharp
public class Author
{
    // 👇 Sollen Ids nicht automatisch erzeugt werden, kann dieses Attribute dies verhindern.
    // Standardmäßig wird eine Identity-Spalte mit Autoincrement in der DB angelegt.
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    // 👇 Validierungsattribute können verwendet werden, um die Konsistenz der Daten sicherzustellen.
    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(255)]
    public string LastName { get; set; }

    [Range(0, 150)]
    public int Age { get; set; }

    // 👇 Relationen werden unterstützt, komplizierte Fälle wie many-to-many (Books) müssen aber noch im DbContext nachkonfiguriert werden
    public ICollection<ContactInfo> ContactInfos { get; set; }
    public ICollection<BookAuthorRel> Books { get; set; }
}
```

Der `DbContext` ist die zentrale Stelle welche die Entitäten zusammenfasst und als Einstiegspunkt in die Verwendung der EF Core API bietet.

- Einstiegspunkt für das EF um das Objektmodell verwalten zu können und damit zu arbeiten
- Entitäten die direkt abgefragt und verarbeitet werden sollten in DbSet<TEntity>-Properties veröffentlicht werden
- Über den Context kann das Modell vor der ersten Verwendung konfiguriert werden
- Der Context bietet Lifecycle-Hooks wie SaveChanges() und ShouldValidateEntity() die überschrieben werden können
- Über den Context wird auch der zu verwendende Data Provider konfiguriert (meist in .config, .json)
- Über den Context kann auf den Change Tracker zugegriffen werden, welcher Änderungen an den Entitäten aufzeichnet
- Über SaveChanges() werden die Änderungen in der DB persistiert
- Enthält einen Cache für bereits geladene Entitäten
  - Daher sollten Entitäten nicht am EF vorbei modifiziert werden

Hier ein Beispiel für den `DbContext`:

```csharp
public class BookDbContext : DbContext
{
    // Die Options enthalten Informationen für die DB-Connection mit der das EF-Framework
    // auf die DB zugreifen soll
    public BookDbContext(DbContextOptions<BookDbContext> options)
        : base(options)
    {
    }

    // Die Entitäten die direkt abgefragt werden können sollen,
    // werden über DbSets angegeben. Es müssen nicht alle Entitäten
    // angegeben werden
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    // Hier können noch Konfigurationen an Entitäten
    // und Conventions durchgeführt werden, bevor
    // das Modell benutzbar ist
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //                                                        👇 EF versucht automatisch den passenden Datentypen für die Tabellenspalte zu erkennen, dies kann hier festgelegt werden.
        modelBuilder.Entity<Book>().Property(b => b.ReleaseDate).HasColumnType("datetime2");
    }
}
```

EF Core hat eine vielzahl von Konventionen die automatisch angewendet werden um möglichst wenig zusätzliche Konfiguration erstellen zu müssen.
Über Attribute in den Entitäten und / oder in `OnModelCreating()` kann das Modell und die angewendeten Konventionen nach Bedarf geändert werden.
