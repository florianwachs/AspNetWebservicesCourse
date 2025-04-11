# Dependency Injection (DI) mit AspNetCore

Da in `aspnetcore` viel mit `Dependency Injection` gearbeitet wird, sollten wir uns die Zeit nehmen das Pattern und dessen Einsatz näher zu betrachten.


```csharp
var services = builder.Services;
// Eigene Services registrieren, welche im DI System verfügbar sein sollen

//       👇 Wenn eine Komponente nach ITimeService verlangt, wird ein Singelton von DefaultTimeService zurückgegeben
services.AddSingleton<ITimeService, DefaultTimeService>();

//       👇 Während der Laufzeit eines Requests wird immer das gleiche IBookRepository an die Komponenten ausgegeben
services.AddScoped<IBookRepository, DummyBookRepository>();

//       👇 Jedesmal wenn eine Komponente ein IBookRepository anfrägt, erhält es eine neue Instanz
services.AddTransient<IBookRepository, DummyBookRepository>();
```

Zur Registrierung stehen uns verschiedene `Lifetimes` zur Auswahl.

| Methode      | Bedeutung                                                                                                                                           |
| ------------ | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| AddSingleton | Registriert einen Typen von dem innerhalb des DI-Containers nur eine einzige Instanz erzeugt wird. Alle Aufrufer erhalten immer das gleiche Objekt. |
| AddScoped    | Während eines Requests durch die Pipeline erhält der Aufrufer immer das gleiche Objekt. Ein neuer Request erhält sein eigenes Objekt.               |
| AddTransient | Jeder Aufrufer bekommt eine neue Instanz                                                                                                            |

DI-Systeme sind haben, wie alles, Vor- und Nachteile.
Einerseits ermögliche sie den einfachen Austausch von Implementierungen z.B. für Unittests. Zum anderen bieten es neue Fehlerquellen wie z.B. nicht registrierte Abhängigkeiten die zur Laufzeit Fehler erzeugen.

## Clean Code

Bei vielen Services die am DI System registriert werden müssen kann es schnell unübersichtlich werden. Die C# [Extension-Methods](https://github.com/florianwachs/AspNetWebservicesCourse/blob/main/00_cheatsheets/csharplanguage/csharp_cheat_sheet.md#extension-methods) können helfen, Services unserer Anwendung gebündelt zu registrieren.

```csharp
public static class DiRegistrationExtensions
{
    //                                                      👇 "this" ist wichtig damit es eine Extension Method ist
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ICountryRepository, CountryRepository>();
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IMachineTypeRepository, MachineTypeRepository>();
        services.AddSingleton<ITenantRepository, TenantRepository>();
        services.AddSingleton<IBranchesOfIndustryRepository, BranchesOfIndustryRepository>();
        services.AddSingleton<IUserSettingRepository, UserSettingRepository>();
        services.AddSingleton<ITopicRepository, TopicRepository>();
        services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
        services.AddSingleton<IMasterDataService, MasterDataService>();
        services.AddSingleton<IMailTaskRepository, MailTaskRepository>();

        return services;
    }
}
```

Damit können wir in unserer `Program.cs` folgendes schreiben.

```csharp
//...
services.AddDataAccessServices();
//...
```
