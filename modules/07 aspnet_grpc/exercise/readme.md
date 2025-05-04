# Übung gRPC

Gegeben ist der Code aus der Vorlesung.

Das Ziel dieser Übung ist, den `SensorService` dahingehend zu erweitern, das das zuletzt erhaltene `SensorReadingPackage` abgefragt werden kann.

Hierzu musst du
- die `WeatherSensor.proto` anpassen
- die Solution kompilieren
- deine neue Methode in `SensorService.cs` durch `override` implementieren

Hinweise:

- gRPC benötigt HTTP/2 was auch zwingend https voraussetzt
- ein gRPC kann nicht einfach `null` zurückgeben
- du kannst beliebig viele `messages` definieren
- eine `message` kann auch andere `message`-Typen als Felder enthalten
- Felder können als `optional` gekennzeichnet werden