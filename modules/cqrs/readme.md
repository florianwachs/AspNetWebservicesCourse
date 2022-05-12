# CQRS| Command-Query Responsibility Segregation

## Was sind die Ziele?

- Trennung von Code der Veränderungen vornimmt und Queries ausführt
- Vorstellbar als Trennung in Write-Service und Read-Service
- Näher an dem, wie Domain Experts ihre Anforderungen beschrieben (diese denken selten in Create-Read-Update-Delete)

Das Pattern selbst ist sehr einfach, ermöglicht aber viele Erweiterungen der Architektur z.B.:
- Erleichtert die Umsetzung von Separation Of Concerns und Open Closed Principal
- Durch die Trennung kann für Read / Write jeweils die optimale Datenlösung (SQL, NoSQL, …) verwendet werden
- Read und Write-Model können optimal auf die Anwendungsfälle angepasst werden, häufig gibt es mehrere Read-Modelle

## Mediator Pattern

- Das Mediator Pattern erlaubt die Interaktion zwischen Objekten zu kapseln und somit zu entkoppeln
- Ermöglicht die Trennung von Query / Command und Handler-Implementierung
- Queries und Commands können so einfache Objekte sein

## MediatR

- CQRS ist problemlos ohne Framework umsetzbar
- Mit MediatR gibt es aber eine leichtgewichtige Bibliothek, welche die Umsetzung von Commands und Queries stark vereinfach
- Vom Macher von Automapper
- https://github.com/jbogard/MediatR/wiki
- In-Memory Message Bus
- Unterstützung für asynchrone Handler
- „Pipeline-Behavior“ für Logging, Analytics, Validierung, Autorisierung usw.
