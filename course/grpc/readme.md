# gRPC

## Ansätze für verteilte APIs

- RPC
- Messaging, Queuing
- COM, DCOM, COBRA, JAVA RMI / .NET Remoting
- SOAP, REST
- GraphQL, ODATA
- gRPC

## gRPC

(Google) Remote Procedure Call

![](assets/grpclogo.png)

- Binäre Kommunikation
- Plattformübergreifend (Java, JavaScript (Node), Python, Go)
- Unterstützung für bidirektionales Streaming
- Nur über HTTPS/2 => TLS/SSL Zwang

## Wie passt gRPC in bestehende Ansätze?

- SignalR: Multicasting, bi-direktionale Kommunikation
- GraphQL: „Web-Query-Language“
- REST: CRUD Web Applikationen
- gRPC: effizientes Streaming / Kommunikation zwischen Service-Komponenten

## Performance

![](assets/grpcperf.png)

## gRPC und procobuf

- gRPC nutzt Protocol Buffers (protobuf)
- Interface Definition Language
- erweiterbar und serialisierbar
- Sprach- und plattformneutral
- Nicht abhängig von gRPC
- Protobuf-net

https://github.com/protocolbuffers/protobuf/releases


