# REST-Guidelines

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

### RESTful URIs und Actions

- Ressourcen sind die "Nouns": customers, orders, tickets, groups
- HTTP-Verben sind die Actions
  - `GET` /customers – Liefert eine Liste von Kunden
  - `GET` /customers/12 – Liefert einen spezifischen Kunden
  - `POST` /customers – Erzeugt eine neue Kunden-Ressource
  - `PUT` /customers/12 – Ersetzt die Daten der Kunden-Ressource
  - `PATCH` /customers/12 – Führt ein Teil-Update auf die Kunden-Ressource aus
  - `DELETE` /customers/12 – Löscht\* die Kunden-Ressource

> GET: verändert niemals Daten oder führt zu einem anderen Zustand der Ressource. Plural für die „Nouns“ hat sich durchgesetzt, da es zu einem API-Endpunkt führt auf den die Actions ausführt werden können

### RESTful URIs und Actions mit Relationen / Verschachtelungen

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


