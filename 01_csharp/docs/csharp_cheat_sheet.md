# C# Cheat Sheet

Dies ist eine Zusammenfassung der Grundlagen der C# Sprache. Während der Vorlesung gehen wir auf alle notwendigen Konstrukte ein. Die [Microsoft Docs](https://docs.microsoft.com) sind eine hervorragende Quelle weiterer Informationen.

## Statements

```csharp
// Sehr ähnlich zu C++ und Java
if (<bool expr>) { ... } else { ... };
switch(<var>) { case <const>: ...; };
while (<bool expr>) { ... };
for (<init>;<bool test>;<modify>) { ... };
do { ... } while (<bool expr>);
```

## Einfache Datentypen

- Integer Types
  - byte, sbyte (8bit), short, ushort (16bit)
  - int, uint (32bit), long, ulong (64bit)
- IEEE Floating Point Types
  - float (precision of 7 digits)
  - double (precision of 15–16 digits)
- Exact Numeric Type
  - decimal (28 significant digits)
- Character Types
  - char (single character)
  - string (rich functionality, by-reference type)
- Boolean Type
  - bool (distinct type, not interchangeable with int)

## Boxing / Unboxing

```csharp
int x = 1;
// Boxing: int => object
object boxDHL = x;
object boxUPS = x;
var gleichesObjekt = object.ReferenceEquals(boxDHL, boxUPS); // false!

// Unboxing object => int
var y = (int)boxDHL;
var z = (int)boxUPS;
```

## Nullable Types

- Nur für Value-Types
  - int?, long?, decimal?, enums
- Damit können fehlende oder ungültige Werte signalisiert werden
- `HasValue`, `GetValueOrDefault` erleichtern den Umgang mit `null`
- Besonders hilfreich beim Laden von Daten aus einer Datenbank oder in einem ViewModel

### unassinged values

Die .NET Runtime initialisiert zwar Variablen, der C#-Compiler betrachtet dies jedoch als (Programmier-)Fehler.

```csharp
int a;
Console.WriteLine(a); // Compilerfehler
```

### default oder assign

Das `default` keyword liefert den Default-Wert eines Typs. Bei numerischen Typen ist dies `0`, bei Referenztypen `null`, bei enums das erste Element und bei structs eine default-initialisierte Instanz.

```csharp
var a = default(int);

// oder ab C# 7.x
a = default;

// oder
a = 0;

Console.WriteLine(a); // Variable ist initialisiert
```

```csharp
int? a = default(int?);
// oder
a = null;
if (a.HasValue)
{
// Foo
}
int aOrDefault = a.GetValueOrDefault(10);

// oder
// null-coalescing operator
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
aOrDefault = a ?? 10;
```

## Variablendeklaration

- Implicit Typed Variable
- Der Compiler setzt den Typ während des Kompilierens
- Der Compiler nimmt bei den simplen Datentypen einen default Typ. Es kann aber mit Literalen wie `m`, `f`, `l` übersteuert werden
- Nicht mit `var` aus JavaScript verwechseln (entspricht eher dem `dynamic` Keyword von C#)

```csharp
public void Explicit()
{
    int a = 0;
    double b = 2.3;
    decimal c = 3.4m;
    string d = "Hallo";

    List<Tuple<int, string, Dictionary<int, string>>> maybeMakeAClass =
        new List<Tuple<int, string, Dictionary<int, string>>>();
}

```

```csharp
public void Var()
{
    var a = 0;
    var b = 2.3;

    // Das m Literal ist nötig um decimal zu erhalten.
    // Als Standard wird double verwendet.

    var c = 3.4m;
    var d = "Hallo";

    var maybeMakeAClass = new List<Tuple<int, string, Dictionary<int, string>>>();
}
```

## Enumerations

- Typisierte Aufzählungen (statt `int`)
- Haben eine implizierte Nummerierung die angepasst werden kann
- können mit boolescher Algebra verwendet werden
- Besonders nützlich in `switch`-Ausdrücken

```csharp
public enum TaskStates
{
    New,        // 0
    Committed,  // 1
    InProgress, // 2
    Done,       // 3
}

public class Task
{
    public TaskStates State { get; set; }

    public Task()
    {
        State = TaskStates.New;
    }
}

// Verwendung in einem switch-Statement
public void SetNewState(TaskStates newState)
{
    switch (newState)
    {
        case TaskStates.New:
            break;
        case TaskStates.Committed:
            break;
        case TaskStates.InProgress:
            break;
        case TaskStates.Done:
            break;
        default:
            break;
    }
}
```

```csharp
// das Flags-Attribut hat nur Auswirkung in der ToString()-Methode
[Flags]
public enum DocumentOptions
{
    ConvertToPDF = 1,
    SendAsMail = 2,
    Archive = 4,
    MarkAsConfidential = 8,
    Default = ConvertToPDF | Archive,
}

// Bit-Operations mit Enums
public void Foo(string documentText, DocumentOptions options)
{
    if ((options & DocumentOptions.ConvertToPDF) != 0)
    {
        // foo
    }

    if (options.HasFlag(DocumentOptions.MarkAsConfidential))
    {
        // send to ... :-)
    }
}
```

## Namespaces

- Dienen der semantischen Code-Strukturierung
- Verhindert Namenskollisionen
- Können geschachtelt werden
- Müssen nicht einer Dateisystem-Struktur folgen
- Können über mehrere Dateien verteilt werden
- über `using [Namespace]` können Namespaces eingebunden werden
  - `using System.Data;`
  - `using System.Collections.Generic;`

```csharp
// Definition
namespace FHRWebservices.Basics
{
    // Klassen, enums, structs, ...
}

// Verwendung
using FHRWebservices.Basics;

// Namespaces können geschachtelt werden
namespace FHRWebservices.Basics
{
    namespace Service
    {
        // Klassen, Structs, Enums
    }

    namespace Service.Tools
    {
        // Können auch geschachtelt werden
        namespace Nested
        {
            // Klassen, Structs, Enums
        }
    }
}

// Best-Practice ist aber auf die Schachtelung zu verzichten
namespace FHRWebservices.Basics.Service
{
    // Klassen, Structs, Enums
}

namespace FHRWebservices.Basics.Service.Tools
{
    // Klassen, Structs, Enums
}

namespace FHRWebservices.Basics.Service.Tools.Nested
{
    // Klassen, Structs, Enums
}

```

## Properties

- Werden vom Compiler in get- und set-Methoden übersetzt
- Automated Properties brauchen kein Backingfield, der Compiler legt eins an
- get und set können unterschiedliche Access-Modifier haben
- Mit C# ab Version 6 können auch Automated Properties schon bei der Definition initialisiert werden
- Ab C# 7 können auch Properties als Expression definiert werden

```csharp
// Properties mit Backing-Field
// Mischung aus Feld und Methode
// get / set oder beides implementieren

private int age;
public int Age
{
    get
    {
        return age;
    }

    private set
    {
        age = value;
    }
}

// auto-property
public string FirstName { get; private set; }

// C# 6: initializer
public DateTime TimeStamp { get; } = DateTime.UtcNow;

// C# 7: expression syntax
public DateTime TimeStamp => DateTime.UtcNow;

public int Age
{
    get => age;
    set
    {
        if (age >= 0)
            age = value;
    }
}
```

## Klassen

- user-defined Type
- Grundbaustein der OOP
- kapselt Fields, Properties, Methods, usw. in einer semantischen Einheit
- mit dem `new`-Keyword wird eine neue Instanz einer Klasse erzeugt
- Die Felder und Properties bilden den Zustand einer Instanz
- Klassen können in Klassen geschachtelt werden
- `static`
  - Klasse: es gibt pro AppDomain nur eine Instanz der Klasse, die Runtime erzeugt diese automatisch. Alle Member müssen ebenfalls static sein
  - Member: Instanz-Member können in einer non-static Klasse mit static Membern gemischt werden
  - Statische Klassen werden nur für Helper / Tools empfohlen, da sie eine enge Koppelung mit sich bringen die in Unit Tests schwer bis nicht auflösbar ist

```csharp
public class Person
{
    // Fields
    private long? id;

    // Properties
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? Created { get; private set; }
    public bool IsNew
    {
        get { return !id.HasValue; }
    }

    // …
}
```

### Konstruktoren

```csharp
public class Person
{
    // …

    // Konstruktoren: Expliziter Default
    public Person()
    {
        Created = DateTime.Now;
    }

    // Custom Konstruktor der den Default-Construktor aufruft
    public Person(string firstName, string lastName) : this()
    {
        FirstName = firstName;
        LastName = lastName;
    }

    // …
}
```

### Methoden

```csharp
public class Person
{
    // …

    // Konstruktoren: Expliziter Default
    public Person()
    {
        Created = DateTime.Now;
    }

    // Custom Konstruktor der den Default-Construktor aufruft
    public Person(string firstName, string lastName) : this()
    {
        FirstName = firstName;
        LastName = lastName;
    }

    // …
}
```

### Verwendung

```csharp
// Default Konstruktor
var noName = new Person();
Console.WriteLine(noName.Created);

// Custom Konstruktor
var hansi = new Person("Hansi", "Hintermeier");

// Object-Initializer Syntax
// https://msdn.microsoft.com/en-us/library/bb384062.aspx
var nice = new Person
{
    FirstName = "Jason",
    LastName = "Bourne"
};
```

## Statische Klassenelemente

```csharp
// Default Konstruktor
var noName = new Person();
Console.WriteLine(noName.Created);

// Custom Konstruktor
var hansi = new Person("Hansi", "Hintermeier");

// Object-Initializer Syntax
// https://msdn.microsoft.com/en-us/library/bb384062.aspx
var nice = new Person
{
    FirstName = "Jason",
    LastName = "Bourne"
};


```

```csharp
public class StaticMemberInClass
{
    // in einer non-static class können static Member enthalten sein
    public static string XmlElementName
    {
        get
        {
            return "StaticMember";
        }
    }
}
```

## Access Modifier

- public : Keine Zugriffsbeschränkung
- private: Sichtbarkeit nur innerhalb des definierenden Types
- protected: Sichtbar innerhalb des deklarierenden Types und Ableitungen der Klasse
- internal: nur innerhalb der definierenden Assembly sichtbar
- internal protected: nur innerhalb der definierenden Assembly oder Ableitungen der Klasse sichtbar
- Können an Typdefinitionen und Membern vergeben werden
- Werden sie weggelassen ist der Standard für Typen internal und für Member private

## Vererbung

- Nur einfache Vererbung (bei C++ ist Mehrfachvererbung möglich)
- `base`
  - am Konstruktor: Konstruktoren der Basisklasse aufrufen
  - in Methode / Property: Implementierung der Basisklasse aufrufen
- `abstract`
  - an der Klasse: es kann keine Instanz dieser Klasse mit new erzeugt werden
  - an einer Methode / Property: eine Ableitung muss eine Implementierung liefern
  - für abstract Methoden / Properties muss auch die Klasse abstract sein
- `virtual`
  - kann an Methoden und Properties definiert werden
  - kann in Ableitungen mit override überschrieben werden
- `sealed`
  -an der Klasse: Es kann nicht weiter von der Klasse abgeleitet werden. Hat einen gewissen Performancevorteil
  -an einer Methode / Property: Der Member kann von Ableitungen nicht weiter überschrieben werden

```csharp
// Nur Einfachvererbung erlaubt
public class Student : Person
{
    public decimal Happiness { get; private set; }

    public void MakeParty()
    {
        Happiness = decimal.MaxValue;
    }

    public void WriteTests()
    {
        Happiness = decimal.MinValue;
    }
}

// ...
var student = new Student
{
    FirstName = "Schakeline Mandy",
    LastName = "Mandy"
};

student.MakeParty();
student.WriteTests();
student.Save();

```

### base

```csharp
public class Student : Person
{

    // Mit base kann auf die Konstruktoren der Basis zugegriffen werden
    public Student(string firstName, string lastName, decimal motivation)
        : base(firstName, lastName)
    {
        Motivation = motivation;
    }

    public override string ToString()
    {
        // mit base kann auf Implementierungen der Basisklasse
        // zugegriffen werden
        return string.Format("{0} {1}:", base.ToString(), MoodStatus);
    }
}
```

## abstract

```csharp
// von abstrakten Klassen kann keine
// Instanz erzeugt werden
public abstract class DtoBase
{
    public abstract string ElementName
    {
        get;
    }

    public abstract Task AppendTo(XmlWriter w);
    public abstract void ReadFrom(XElement e);
}

// Implementierung der abstrakten Klasse

public class ArticleDto : DtoBase
{
    public override string ElementName
    {
        get {// …}
    }

    public override Task AppendTo(XmlWriter w)
    {
        // …
    }

    public override void ReadFrom(XElement e)
    {
        // …
    }
}

```

### virtual

```csharp
public abstract class DtoBase
{
    // virtual Methoden bieten eine Implementierung
    // die aber von Ableitungen überschrieben werden
    // können
    public virtual string GetDebugMessage()
    {
        return "Type: " + GetType().Name;
    }
}

public class ArticleDto : DtoBase
{
    public override string GetDebugMessage()
    {
        return string.Format("{0} {1}", base.GetDebugMessage(), Name);
    }
}
```

### sealed

```csharp
// sealed unterbindet weitere Ableitungen
public sealed class ArticleDto : DtoBase
{
}

public class ArticleDto : DtoBase
{
    // sealed unterbindet weiteren override in Ableitungen
    public sealed override string GetDebugMessage()
    {
        return string.Format("{0} {1}", base.GetDebugMessage(), Name);
    }
}

```

## Interfaces

- Beschreibt ein Verhalten, dass von der implementierenden Struktur oder Klasse erfüllt werden muss
- enthalten keine Implementierung, nur eine Schnittstellenbeschreibung
- class und struct können beliebig viele Interfaces implementieren
- können von anderen Interfaces ableiten
- die .NET Base Class Library (BCL) wird mit hunderten Interfaces ausgeliefert, z.B. IEnumerable<>, IComparable, IEquatable<>
- Essentiell für eine Vielzahl von Design Patterns

```csharp
public interface IStudentRepository
{
    IEnumerable<Student> GetAll();
    Student GetById(int id);
}


public static void PrintStudentsMood(IStudentRepository repo)
{
    foreach (var student in repo.GetAll())
    {
        Console.WriteLine(student.MoodStatus);
    }
}

public static void Demo1()
{
    PrintStudentsMood(new StudentsDuringTestRepository());
    PrintStudentsMood(new StudentsDuringPartyRepository());
}

// Einsatz im Repository-Pattern
public class StudentsDuringTestRepository : IStudentRepository
{
    public IEnumerable<Student> GetAll()
    {
        return students;
    }

    public Student GetById(int id)
    {
        return students.FirstOrDefault(student => student.Id == id);
    }
}

```

## Casting

```csharp
public static void BasicTypes()
{
    int i = 5;
    // wenn von einem Typ mit kleinerem Wertebereich
    // in einen mit größeren zugewiesen wird,
    // ist kein explizites Casting nötig
    long l = i; //long l = (long)i;

    // wenn von einem Typ mit größerem Wertebereich
    // in einen mit kleineren zugewiesen wird,
    // ist ein explizites Casting nötig
    // ACHTUNG: dabei geht bestenfalls Genauigkeit verloren
    double d = 1.2;
    float f = (float)d;
}

public static void ReferenceTypes()
{
    // Student : Person
    var student = new Student();

    // implicit upcast
    Person p = student;

    // wirft einen Fehler
    // student = p;

    student = (Student)p;
}

```

### as / is

- Für die ValueTypes im .NET Framework sind bereits sinnvolle implizite und explizite casts definiert
- Es können auch eigene Type-Conversions in eigenen Typen definiert werden
- Mit is und as können Typ-Conversions erst geprüft und „sicher“ durchgeführt werden

```csharp
public static void AsIs()
{
    IStudentRepository repo = new StudentsDuringPartyRepository();

    try
    {
        var fail = (StudentsDuringTestRepository)repo;
    }
    catch (InvalidCastException)
    {
    }

    // mit is kann getestet werden, ob eine Typkonvertierung erfolgreich wäre
    Console.WriteLine(repo is StudentsDuringTestRepository); // false

    // mit as kann eine Typkonvertierung durchgeführt werden
    // wenn die Typen nicht kompatibel oder null sind, wird auf null evaluiert
    var dasIstNull = repo as StudentsDuringTestRepository;
}
```

## structs

- Können auch Interfaces implementieren
- Sind sealed, dh. Es kann nicht weiter von ihnen abgeleitet werden
  Value-Type
- Werden oft für mathematische Konstrukte verwendet (Point, Money, Vector) wo die Eigenschaften der Value-Types „natürlicher“ als die der Reference-Types sind

```csharp
public struct PointStruct : ICanMove
{
    public int X;
    public int Y;

    public PointStruct(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public void MoveBy(int x, int y)
    {
        X += x;
        Y += y;
    }

    public override string ToString()
    {
        return string.Format("X:{0}, Y:{1}", X, Y);
    }
}
```

## BY-VALUE VS. BY-REFERENCE

- Argumente werden standardmäßig by-value übergeben
- Eine Kopie des Wertes wird erzeugt, wenn das Argument an die Methode übergeben wird
  - Bei Value-Types: eine komplette Kopie der Struktur
  - Bei Reference-Types: nur die Referenz auf das Objekt wird kopiert

```csharp
private static void FooByValue(Student s)
{
    // s ist eine Kopie der Referenz auf s
    // das Objekt lässt sich modifizieren
    s.LastName = "Blaa";

    // das wird nicht funktionieren,
    // da nur die lokale Referenz auf ein
    // neues Objekt zeigt
    s = new Student { FirstName = "HAHAHAHA" };
    Console.WriteLine("Innerhalb von Foo: " + s.ToString());
}

var student = new Student { FirstName = "Liese", LastName = "Müller" };
Console.WriteLine("Student vor Foo(): " + student.ToString()); // Liese Müller
FooByValue(student);
Console.WriteLine("Student nach Foo(): " + student.ToString()); // Liese Blaa
```

```csharp
private static void FooByReference(ref Student s)
{
    // s ist eine Kopie der Referenz auf s
    // das Objekt lässt sich modifizieren
    s.LastName = "Blaa";

    // das wird nicht funktionieren,
    // da nur die lokale Referenz auf ein
    // neues Objekt zeigt
    s = new Student { FirstName = "HAHAHAHA" };
    Console.WriteLine("Innerhalb von Foo: " + s.ToString());
}

var student = new Student { FirstName = "Liese", LastName = "Müller" };
Console.WriteLine("Student vor Foo(): " + student.ToString()); // Liese Müller
FooByReference(ref student);
Console.WriteLine("Student nach Foo(): " + student.ToString()); // HAHAHAHA
```

- Mit dem ref-Modifier wir angegeben, das die Parameterübergabe by-reference erfolgen soll
- Der ref-Parameter muss sowohl bei der Methodendeklaration als auch beim Aufruf angegeben werden
- Beim Aufruf der Methode muss die übergebene Referenz initialisiert sein

```csharp
private static bool SplitNames(string text, out string[] names)
{
    // Muss vor dem Verlassen der Methode zugewiesen werden
    names = null;

    if (!string.IsNullOrWhiteSpace(text))
    {
        names = text.Split();
    }

    return names != null && names.Length > 0;
}

var test = "Hans Meiser";
string[] names;
if (SplitNames(test, out names))
{
    // foo
}

```

- Der out-Modifier verhält sich wie der ref-Modifier, bis auf:
  - Der Parameter darf noch nicht zugewiesen worden sein
  - Muss innerhalb der Methode zugewiesen werden, bevor die Methode verlassen wird
- Viele BCL-Typen machen sich out zunutze
  - DateTime.TryParse, decimal.TryParse, Dictionary.TryGetValue

## Delegates

- Ein Objekt das „weiß“ wie eine Methode aufzurufen ist
- Ein Delegate-Type definiert die Signatur der aufzurufenden Methode
- return type und Parameter der Methode
- Delegates sind typsicher und können als Parameter eingesetzt werden
- Analog zu einem C-function-pointer

```csharp
// Definition eines Delegate-Types
public delegate decimal MathOperation(decimal x, decimal y);

public static decimal Add(decimal x, decimal y)
{
    return x + y;
}

public static decimal Multiply(decimal x, decimal y)
{
    return x * y;
}
// Dem Delegate kann jede Methode zugewiesen werden,
// solange sie der Methodensignatur folgt, die der
// Delegate vorgibt
MathOperation op = new MathOperation(Add);
// oder
op = Add;

Console.WriteLine(op(2, 4)); // 6

op = Multiply;
Console.WriteLine(op(2, 4)); // 8
```

- Die BCL liefert eine Reihe von generellen Delegates mit
- Func<>: n-Parameter und ein Rückgabewert
- Action<>: n-Parameter und void Rückgabe
- delegate TResult Func < out TResult > ();
- delegate TResult Func < in T, out TResult > (T arg);
- delegate TResult Func < in T1, in T2, out TResult > (T1 arg1, T2 arg2);
- delegate void Action ();
- delegate void Action < in T > (T arg);
- delegate void Action < in T1, in T2 > (T1 arg1, T2 arg2);
- Geht bis T16

```csharp
public static decimal Add(decimal x, decimal y)
{
    return x + y;
}

// Generic Delegates
// 2 decimal Parameter, decimal Return Type
Func<decimal, decimal, decimal> op = Add;
Console.WriteLine(op(2, 4));
```

## Lambda-Expressions

- Eine unbenannte Methodenimplementierung statt einer Delegate-Instanz
- „Syntactic Sugar“, Compiler macht die Arbeit
- (parameters) => expression-or-statement-block
- Expression-Block
  - Func<int,int> sqrt = x => x\*x;
- Statement-Block
  - Func<int,int> sqrt= x=>{return x\*x;};
- LINQ und Lambdas sind füreinander gemacht…

```csharp
MathOperation op = (x, y) => x + y;
Console.WriteLine(op(2, 4)); // 6

// oder mit Typ Angabe
MathOperation op2 = (decimal x, decimal y) => x + y;

// mit Generics
Func<decimal, decimal, decimal> op3 = (x, y) => x + y;

// Collections und Arrays bieten viele Methoden die
// einen Delegate als Parameter verwenden
var a = new List<int> { 1, 2, 3, 5, 67, 345, 223334 };
var biggerThan5 = a.FindAll(n => n > 5);


// ohne Lambda
var biggerThan5OhneLambda = a.FindAll(BiggerThan5);

private bool BiggerThan5(int n)
{
    return n > 5;
}
```

## Arrays

- Arrays fassen eine feste Anzahl von Elementen eines Typs zusammen
- Elemente des Arrays werden in einem zusammenhängenden Speicherblock abgelegt, was einen effektiven - Zugriff erlaubt
- Index ist 0-basiert
- Arrays leiten von System.Array ab und bieten einige nützliche Methoden und Eigenschaften

```csharp
// Arrays werden mit ihrer Größe initialisiert
int[] a = new int[5];

// über einen Indexer kann auf Elemente zugegriffen werden
a[0] = 1;
a[2] = 3;

// der Index ist 0-basiert
var third = a[2];

// array initialization expression
int[] b = { 5, 4, 3, 2, 1 };
```

### rectangular arrays

```csharp
var rectangular = new int[3, 3]
{
    { 0, 1, 2 },
    { 3, 4, 5 },
    { 6, 7, 8 }
};
var valueFirstRowSecondColumn = rectangular[0, 1]; // 1

```

### jagged arrays

```csharp
var jagged = new int[3][]
{
    new []{1,2,3},
    new []{4,5},
    new []{6}
};

var val = jagged[1][1]; // 5
```

## Collection

```csharp
var list = new List<int>();
list.Add(1);
list.AddRange(new[] { 2, 3, 4, 5 });

for (int i = 0; i < list; i++)
{
    var item = list[i];
    Console.WriteLine(item);
}

foreach (var item in list)
{
    Console.WriteLine(item);
}
```

```csharp
var map = new Dictionary<string, decimal>
{
    { "EUR", 1 },
    { "USD", 1.1m },
    { "NOK", 8m }
};

map.Add("CHF", 2);

var quote = map["CHF"];
map["CHF"] = 2.5m;

if (map.ContainsKey("AUD")) { }

decimal q;
if (!map.TryGetValue("AUD", out q))
{
    q = 10;
}

// C# 7: out-Variables
if(map.TryGetValue("USD", out decimal amount))
{
    q = amount;
}

```

## LINQ

- Language Integrated Query
- Framework und Sprachfeatures um typsichere Queries gegen Collections und Arrays schreiben zu können
- (alles was IEnumerable<T> implementiert)
- Filterungs- und Aggregierungsmöglichkeiten

### Fluent-Syntax

```csharp
var test = Enumerable.Range(0, 50);

var methodQuery = test
    .Where(number => (number % 2) == 0)
    .Select(number => Math.Pow(number, 2));

foreach (var number in methodQuery)
{
    Console.Write(number + " ");
}
```

### Query-Syntax

```csharp
var test = Enumerable.Range(0, 50);

var query = from number in test
            where (number % 2) == 0
            select Math.Pow(number, 2);

foreach (var number in query)
{
    Console.Write(number + " ");
}
```

> Beide Queries werden erst ausgeführt, wenn iteriert wird

```csharp
IStudentRepository repo = new StudentsDuringPartyRepository();

foreach (var student in repo.GetAll()
    .Where(student =>
        student.Motivation > 5.0 && student.FirstName.StartsWith("A"))
        .OrderBy(student => student.FirstName))
{
    // Foo
}
```

## Indexer

- Array-like Zugriff auf Klassen und Strukturen die sich wie eine Liste oder ein Dictionary verhalten
- Auch string implementiert einen Indexer: var test = „Hallo“; char a = test[1];

```csharp
// Aus C# in a Nutshell
public class Sentence
{
    private string[] words;

    public Sentence(string text)
    {
        words = text.Split();
    }

    public string this[int index]
    {
        get { return words[index]; }
        set { words[index] = value; }
    }

    public override string ToString()
    {
        return string.Join(" ", words);
    }
}

var t = new Sentence("The quick brown fox");
Console.WriteLine(t[2]); // quick
t[2] = "old";
Console.WriteLine(t.ToString());

```

## Attributes

-.NET erlaubt das Hinzufügen von zusätzlichen Metadaten zu Types, Members und Assemblies
-Eigener Code kann damit „dekoriert“ werden
-Die BCL hat eine Reihe definiert -[Serializable], [Test], [Obsolete], [Flags]
-Ein Attribut alleine hat keine Wirkung, ausführender Code (oder die Runtime) muss die Attibute berücksichtigen

```csharp
// Definition

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TaskAttribute : Attribute
    {
        public enum Severitiy { Low, Mid, High, Critical }

        public string Description { get; set; }
        public Severitiy Level { get; set; }

        public TaskAttribute(string description)
        {
            this.Description = description;
        }
    }

// Verwendung

// Per Konvention kann man statt TaskAttribute auch nur Task schreiben
[Task("Hier fehlt die Implementierung", Level = TaskAttribute.Severitiy.Critical)]
[Task("Wofür ist die Klasse da?")]
public class MyClass
{
}

// Auswertung
var type = typeof(MyClass);
var attribs = type.GetCustomAttributes(typeof(TaskAttribute), true);

if (attribs.Length != 0)
{
    foreach (var attrib in attribs)
    {
        var taskAtrib = (TaskAttribute)attrib;
        Console.WriteLine("Task '{0}' hat die Dringlichkeit {1}",
            taskAtrib.Description,
            taskAtrib.Level);
    }
}

```

## Generics

- Oberstes Ziel: Code Wiederverwendbarkeit
- Sind ein Template mit Typ-Platzhaltern
- Macht die C# Collections performant und typsicher
- Können in Interfaces, classes, Methoden und structs definiert werden
- Gleiche Logik, nur unterschiedliche Typen
- Constraints können für jeden Typ-Parameter angegeben werden, egal ob in Typdefinition oder - Methodendefinition

### class

```csharp
 public class MyAwesomeStack<T>
{
    private const int Max_Size = 100;

    private int position;
    private T[] data = new T[Max_Size];

    public void Push(T item)
    {
        data[position++] = item;
    }

    public T Pop()
    {
        return data[position--];
    }

}

var myStringStack = new MyAwesomeStack<string>();
myStringStack.Push("hello");

var myIntStack = new MyAwesomeStack<int>();
myIntStack.Push(1);

```

### method

```csharp
private static void Swap<T>(ref T a, ref T b)
{
    T temp = a;
    a = b;
    b = temp;
}

int x = 5;
int y = 10;

// man kann die Typenliste angeben
Swap<int>(ref x, ref y);

// oder es den Compiler machen lassen
Swap(ref x, ref y);

```

### constraints

- where T: base-class
  - Base Class Constraint
- where T: interface
  - Interface Constraint
- where T: class
  - Reference-Type Constraint
- where T: struct
  - Value-Type Constraint
- where T: new()
  - Parameterless Constructor Constraint
- where U: T

```csharp
public class MyFactory<T> where T : new()
{
    public T[] Make(int count)
    {
        return Enumerable.Range(0, count).Select(_ => new T()).ToArray();
    }
}

public class MyGenericClass<T, U>
    where T : Student, ICanMove
    where U : new()
{
    // T muss von Student ableiten oder Student sein und das ICanMove Interface implementieren
    // U muss einen parameterlosen Konstruktor besitzen
}

```

## Exceptions

- Es können eigene Ableitungen von Exceptions erzeugt werden
- Das .NET Framework definiert eine Vielzahl: FileNotFoundException, InvalidOperationException, - NotImplementedException, StackOverflowException
- Können innerhalb eines try-catch Blocks behandelt werden
- Es können beliebig viele catch-Blöcke angegeben werden
- Mit throw kann eine Exception weitergeworfen werden
- Exceptions können in andere Exceptions verpackt werden
- Bad Practice: Exceptions nie zur Programmflusssteuerung verwenden

```csharp
try
{
    NotDoneYet();
}
catch (InvalidOperationException)
{
    // Fängt nur Exceptions von diesem Typ
}
catch (Exception ex)
{
    // Fängt alle restlichen Exceptions

    // wirft die Exception weiter den Stacktrace rauf
    throw;
}
finally
{
    // Wird auf "jeden" Fall ausgeführt
    // Aufräumen, z.B. File-Handles schließen
}

public void NotDoneYet()
{
    throw new NotImplementedException("Hab´s doch gesagt!");
}

```
