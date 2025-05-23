# Security Policies Übung

Gegeben ist der Code aus der Vorlesung.

## Aufgabe 1

- [ ] Stelle sicher, dass der Code bei Dir kompiliert und ausgeführt werden kann.

## Aufgabe 2

- [ ] Definiere in `AuthorEndpoints.cs` einen neuen `Delete`-Endpunkt mit dem ein Author entfernt werden kann.
Es muss nur ein geeigneter Status-Code zurückgeliefert werden, es muss nichts wirklich gelöscht werden da wir nur die Security betrachten.

z.B.:

```csharp
app.MapDelete("/api/v1/authors/{id:int}", () =>
{
  return Results.???;
});
```

## Aufgabe 3

Ein Autor darf nur von einem Content-Manager entfernt werden.
Bitte
- [ ] erweitere den `AppUser` entsprechend um eine passende Property
- [ ] erstelle eine Migration welche die neue Property beinhaltet (Hinweis `dotnet ef migrations`)
- [ ] definiere eine geeignete Policy
- [ ] schütze den Endpunkt
- [ ] lösche die `app.db` und definiere in `EnsureUsers` einen neuen User der ein Content-Manager ist
- [ ] teste den Endpunkt mit Open API / Swagger oder einen HTTP Tool deiner Wahl
