# Dependency Injection (DI) mit AspNetCore

Da in `aspnetcore` viel mit `Dependency Injection` gearbeitet wird, sollten wir uns die Zeit nehmen das Pattern und dessen Einsatz näher zu betrachten.

```csharp
public class NoDIBookController : ControllerBase
{
    public NoDIBookController()
    {
        BookRepository = new DummyBookRepository();
        TimeService = new DefaultTimeService();
    }

    public IBookRepository BookRepository { get; }
    public ITimeService TimeService { get; }

    [HttpGet]
    public IEnumerable<Book> GetAllBooks()
    {
        var currentTime = TimeService.Now;
        return BookRepository.All();
    }
}

```

Hier ist ein Controller, der zur Bewältigung seiner Aufgaben ein BookRepository und einen TimeService benötigt. Diese muss sich der Controller selbst erzeugen. Der Controller ist also für die Erzeugung (und Entsorgung) seiner Abhängigkeiten verantwortlich.
Ein Nachteil von diesem Ansatz ist, dass nun eine enge Koppelung zwischen dem Controller und seinen Abhängigkeiten existiert. Diese können nun nicht mehr ohne weiteres gegen andere Implementierungen ausgetauscht werden. Dies ist insbesondere für Unittests ein Problem.

Eine andere Möglichkeit ist, dass der Controller selbst nur definiert, welche Abhängigkeiten er benötigt und dies dem Verwender auch kommuniziert.

```csharp
public class DIBookController : ControllerBase
{
    public DIBookController(IBookRepository bookRepository, ITimeService timeService)
    {
        BookRepository = bookRepository;
        TimeService = timeService;
    }

    public IBookRepository BookRepository { get; }
    public ITimeService TimeService { get; }

    [HttpGet]
    public IEnumerable<Book> GetAllBooks()
    {
        var currentTime = TimeService.Now;
        return BookRepository.All();
    }

}
```

Diese Version des Controllers fordert seine Abhängigkeiten klar als Constructor-Argument ein. Nun ist der Aufrufer dafür verantwortlich diese bereitzustellen.
Und genau hier kommt das DI-System von `aspnetcore` ins Spiel. Rufen wir den API-Endpunkt nun auf, bekommen wir eine Fehlermeldung:

![DI Exception](assets/di_exception.png)

Die Fehlermeldung sagt aus, das `aspnetcore` nicht in der Lage ist, eine Instanz von IBookRepository an den Controller zu liefern.
Wir müssen Komponenten, welche im DI-System verfügbar sind, registrieren.

Dazu müssen wir folgendes in der `Startup.cs` erweitern:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Eigene Services registrieren welche im DI System verfügbar sein sollen

        //       👇 Wenn eine Komponente nach ITimeService verlangt, wird ein Singelton von DefaultTimeService zurückgegeben
        services.AddSingleton<ITimeService, DefaultTimeService>();

        //       👇 Während der Laufzeit eines Requests wird immer das gleiche IBookRepository an die Komponenten ausgegeben
        services.AddScoped<IBookRepository, DummyBookRepository>();

        //       👇 Jedesmal wenn eine Komponente ein IBookRepository anfrägt, erhält es eine neue Instanz
        services.AddTransient<IBookRepository, DummyBookRepository>();
    }
}

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

Damit können wir in unserer `Startup.cs` folgendes schreiben.

```csharp
 public void ConfigureServices(IServiceCollection services)
{
    //...
    services.AddDataAccessServices();
    //...
```
