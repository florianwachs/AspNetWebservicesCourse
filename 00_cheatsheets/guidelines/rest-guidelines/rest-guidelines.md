# REST-Guidelines

- [REST-Guidelines](#rest-guidelines)
  - [Ursprung von REST](#ursprung-von-rest)
  - [Kernkomponenten von Rest](#kernkomponenten-von-rest)
    - [Resources (URIs)](#resources-uris)
    - [HTTP Verbs](#http-verbs)
    - [HTTP Status Codes](#http-status-codes)
    - [Stateless](#stateless)
  - [RESTful URIs und Actions](#restful-uris-und-actions)
    - [Relationen / Verschachtelungen](#relationen--verschachtelungen)
    - [Außerhalb von CRUD](#außerhalb-von-crud)
  - [Dokumentation der API](#dokumentation-der-api)
  - [Versionierung der API](#versionierung-der-api)
  - [Filtern, Suchen, Sortieren](#filtern-suchen-sortieren)
  - [Ressource nach Aktion wieder zurückgeben](#ressource-nach-aktion-wieder-zurückgeben)
  - [JSON Property Serialisierung](#json-property-serialisierung)
  - [Pagination](#pagination)
  - [HTTP-Verb Override](#http-verb-override)
  - [Rate Limit](#rate-limit)
  - [Zu bedenken](#zu-bedenken)
  - [Ressourcen](#ressourcen)

> Im folgenden werden einige Guidelines / Best Practices für REST-Schnittstellen vorgestellt.
> Mir ist wichtig, darauf hinzuweisen das REST kein zertifizierter Standard ist, sondern dies Guidelines
> viel mehr auf Erfahrungen basieren. Manche dieser Guidelines machen für Euren Anwendungsfall evtl. keinen Sinn
> oder verkomplizieren die Schnittstelle. Wichtig ist, wie bei bei allem, dass Ihr kritisch prüft ob
> eine Guideline für Euch passt oder nicht. Sie wurden aufgebaut um die Schnittstellenentwicklung zu
> erleichtern und sind nicht "als in jedem Fall zu befolgen" vorgesehen.

## Ursprung von REST

**Representational State Transfer (REST)** wurde von Roy Fielding als Teil seiner Dissertation (2000)
entwickelt (https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm)

## Kernkomponenten von Rest

### Resources (URIs)

- "Nouns" der Schnittstelle
- z.B. `customers`, `articles`, `tickets`

### HTTP Verbs

- "Was soll passieren"
- `GET`, `POST`, `PUT`, `DELETE`, `PATCH`
- auch die Definition eigener Verbs ist möglich aber nicht empfohlen
- https://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html
- https://de.wikipedia.org/wiki/Representational_State_Transfer

### HTTP Status Codes

- Antwort des Servers auf die Anfrage des Clients
- `200 OK`, `201 CREATED`, `404 NOT FOUND`, `500 INTERNAL SERVER ERROR`
- https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html

### Stateless

REST Schnittstellen sind **zustandlos** d.h. sie speichern keinen Zustand der Schnittstelle über Requests hinweg. Viel mehr enthält jeder Request alle benötigten Daten (inkl. Authentifizierung) um die Aktion am Server auszuführen.

## RESTful URIs und Actions

- Ressourcen sind die "Nouns": customers, orders, tickets, groups
- HTTP-Verben sind die Actions
  - `GET` /customers – Liefert eine Liste von Kunden
  - `GET` /customers/12 – Liefert einen spezifischen Kunden
  - `POST` /customers – Erzeugt eine neue Kunden-Ressource
  - `PUT` /customers/12 – Ersetzt die Daten der Kunden-Ressource
  - `PATCH` /customers/12 – Führt ein Teil-Update auf die Kunden-Ressource aus
  - `DELETE` /customers/12 – Löscht\* die Kunden-Ressource

> GET: verändert niemals Daten oder führt zu einem anderen Zustand der Ressource. Plural für die „Nouns“ hat sich durchgesetzt, da es zu einem API-Endpunkt führt auf den die Actions ausführt werden können.

### Relationen / Verschachtelungen

- `GET` /customers/12/orders – Liefert eine Liste von Aufträgen für den Kunden 12
- `GET` /customers/12/orders/2 – Liefert den Auftrag mit der Id 2 für den Kunden 12
- `POST` /customers/12/orders – Erzeugt einen neuen Auftrag für den Kunden 12
- `PUT` /customers/12/orders/2 – Ersetzt die Daten des Auftrags mit der Id 2
- `PATCH` /customers/12/orders/2 – Führt ein Teil-Update auf den Auftrag aus
- `DELETE` /customers/12/orders/2 – Löscht\* den Auftrag mit der Id 2

> _Löschen_ mit `DELETE` ist in Softwareanwendung meist eher ein deaktivieren des Datensatzes.

> Es wird empfohlen, nicht tiefer als 2 Ebenen zu schachteln
> z.B: `customers/1/orders/123/orderitems`
> Da REST beliebig viele Ressourcen erlaubt, ist das Problem durch einen eigenen Endpunkt leicht zu lösen `orders/123/orderitems`. Aber auch hier gilt, des es sich nur um eine Richtline handelt die sich bewährt hat.

### Außerhalb von CRUD

Manche Aktionen lassen sich nicht leicht in die RESTful-Struktur übersetzen, z.B. Favoritenmarkierung von Büchern.

> Wichtig: Nicht in den RPC-Style zurück fallen
> Don`t: PUT /books/1/setasfavorite=true

- Alternative
  - isFavorite als Feld in der Ressource aufnehmen und mit einem PUT aktualisieren
- Alternative 2
  - `POST` oder PUT /books/1/favorites
  - `DELETE` /books/1/favorites

## Dokumentation der API

- Gute APIs sind immer dokumentiert
- Welche Möglichkeiten bietet die API?
- Was sind die Ressourcen?
- Zu welchem Ergebnis führen die HTTP-Verben wenn sie auf Ressourcen angewendet werden?
- Was sind die Änderungen zwischen Versionen? (Changelog)
- Wann werden alte API Versionen deaktiviert?
- Beispiel: https://developer.github.com/v3/

## Versionierung der API

Gründe für Versionierung:

- URIs sollten über einen großen Zeitraum hinweg gültig bleiben
- Konsumenten der API können evtl. nicht sofort auf Änderungen der Schnittstelle reagieren
- Alte Versionen können nach und nach abgeschaltet werden

- Möglichkeiten der Versionierung
- Im HTTP-Header der Requests Content Type: application/vnd.github.v3+json oder x-myApp-version: 2
- In der URL api/v3/books, api/v3.0/books
- Als URI-Parameter /api/books?v=2
- Beispiel: https://developer.github.com/v3/

> Alle werden kontrovers diskutiert => Hauptsache es wird versioniert

Unterstützung in ASPNET CORE für Versionierung:

- Von Hand über Routes
  - `[Route("api/v1/[controller]")]`
- Microsoft.AspNetCore.Mvc.Versioning
  - https://github.com/microsoft/aspnet-api-versioning/wiki
- Swagger Integration
- Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer
  - https://github.com/microsoft/aspnet-api-versioning/tree/master/samples/aspnetcore/SwaggerSample

## Filtern, Suchen, Sortieren

Filterung

```http
GET /orders?state=completed
```

Sortierung

```http
GET /orders?sort=createdDate
```

Suchen

```http
GET /customers?q=Maier
```

Kombination

```http
GET /customers?q=Maier&state=new&sort=created,firstname
```

Liefert Kunden deren Name Maier enthält, im Zustand New sind und sortiert diese nach created und firstname

> Nicht mit den Filtermöglichkeiten übertreiben.
> Manchmal braucht man eine Suchtechnologie wie Lucene oder ODATA >(wird noch vorgestellt)
> Für häufige Queries sollten Aliase erstellt werden z.B. GET /orders/recently_delivered

## Ressource nach Aktion wieder zurückgeben

- HTTP-Verben die Ressourcen erzeugen oder verändern sollten diese als Ergebnis an den Aufrufer zurückgeben.
- POST /customers => liefert den neu erzeugten Kunden an den Aufrufer zurück mit Status Code `201 Created` und einem `Location`-Header der die URI auf den neuen Kunden enthält.
- PUT oder PATCH /customers/1 => liefert den veränderten Kunden an den Aufrufer zurück.

## JSON Property Serialisierung

- PascalCase
  - Ist untypisch und sollte nicht verwendet werden
  - FirstName
- CamelCase
  - Häufig eingesetzt
  - firstName
- SnakeCase
  - Findet vermehrt bei neueren APIs Verwendung
  - first_name

> Am besten eine JSON-Library verwenden die eine Modifizierung der Property-Serialisierung erlaubt oder standardmäßig CamelCase nutzt.

## Pagination

Bei zu vielen Ergebnissen über die API muss auf Pagination zurückgegriffen werden.

Häufig enthält die JSON-Response vom Server ein spezielles Paging-Element, welches die URI zur nächsten „Datenseite“ enthält. Bei Bedarf kann der Client diese dann aufrufen.

Moderne Alternative: Link-Header

- https://developer.github.com/v3/#pagination
- Link Header Field https://tools.ietf.org/html/rfc5988#page-6

## HTTP-Verb Override

Manche Firmen lassen durch die Firewall nur GET und POST Requests
Mit dem HTTP-Header `X-HTTP-Method-Override` kann dem Server trotzdem mitgeteilt werden, welche Methode eigentlich gemeint war.

> Niemals mit GET-Requests machen, GET ändert nie den Zustand einer Ressource.

```http
POST /api/Person/4 HTTP/1.1
Host: localhost:10320
Content-Type: application/json
X-HTTP-Method-Override: PUT
Cache-Control: no-cache
```

## Rate Limit

- Besonders public APIs limitieren die Zugriffe der Clients
  - Zugriffe insgesamt
  - Zugriffe in einem bestimmten Zeitraum
- Wichtig ist, dem Client mitzuteilen, dass er ein Limit überschritten hat und wann er wieder auf die API - zugreifen kann
- Häufig verwendete Response-Header:
  - X-Rate-Limit-Limit: Erlaubte Anzahl der Requests
  - X-Rate-Limit-Remaining: Noch verbleibende Menge an Requests
  - X-Rate-Limit-Reset: Sekunden bis das Limit zurückgesetzt wird

## Zu bedenken

- Caching
  - HTTP hat Caching eingebaut, mit Ansätzen wie ETag und Last-Modified können sie für APIs gut nutzbar gemacht - werden
  - https://en.wikipedia.org/wiki/HTTP_ETag
  - https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.29
- SSL
  - APIs mit SSL verschlüsseln, um Inhalt der Kommunikation zu schützen und sicherzustellen das der Inhalt nicht - verändert wurde
- Authentifizierung und Autorisierung
  - APIs werden in den meisten Fällen durch Technologien wie OAUTH geschützt (kommt noch in der Vorlesung)
- Komprimierung
  - Datenpakete sollten auf jeden Fall vom Webserver mit Technologien wie `gzip` oder `brotli` komprimiert werden. Extreme - Einsparungsmöglichkeiten
- Fehlerbehandlung
  - Dem Client über Status Codes mitteilen wo die Ursache liegt. 4xx = Client-Error, 5xx Server-Error
  - Als Content der Response sollte eine verständliche Fehlermeldung im JSON-Format übertragen werden

## Ressourcen

- https://de.wikipedia.org/wiki/Roy_Fielding
- https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design
- http://www.vinaysahni.com/best-practices-for-a-pragmatic-restful-api
- https://developer.github.com/
- https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
