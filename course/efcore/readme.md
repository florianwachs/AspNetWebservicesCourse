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