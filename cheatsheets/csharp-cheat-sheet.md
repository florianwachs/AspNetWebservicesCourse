# C# 13 / .NET 10 — Comprehensive Cheat Sheet

> **Course:** ASP.NET Web Services 2026
> **Runtime:** .NET 10 · **Language:** C# 13
> **Last updated:** 2026-02

---

## Table of Contents

1. [Basic Data Types](#1-basic-data-types)
2. [Boxing / Unboxing](#2-boxing--unboxing)
3. [Nullable Types](#3-nullable-types)
4. [Variable Declaration](#4-variable-declaration)
5. [Enumerations](#5-enumerations)
6. [Namespaces](#6-namespaces)
7. [Properties](#7-properties)
8. [Classes](#8-classes)
9. [Static Members](#9-static-members)
10. [Access Modifiers](#10-access-modifiers)
11. [Inheritance](#11-inheritance)
12. [Interfaces](#12-interfaces)
13. [Casting](#13-casting)
14. [Structs](#14-structs)
15. [Value vs Reference Types](#15-value-vs-reference-types)
16. [Delegates and Events](#16-delegates-and-events)
17. [Lambda Expressions](#17-lambda-expressions)
18. [Arrays](#18-arrays)
19. [Collections](#19-collections)
20. [LINQ](#20-linq)
21. [Generics](#21-generics)
22. [Exception Handling](#22-exception-handling)
23. [Expression-bodied Members](#23-expression-bodied-members)
24. [Null-Conditional and Null-Coalescing Operators](#24-null-conditional-and-null-coalescing-operators)
25. [String Interpolation and Raw String Literals](#25-string-interpolation-and-raw-string-literals)
26. [Anonymous Types](#26-anonymous-types)
27. [Tuples](#27-tuples)
28. [Pattern Matching](#28-pattern-matching)
29. [Records](#29-records)
30. [Top-level Statements](#30-top-level-statements)
31. [Required Members](#31-required-members)
32. [Extension Methods](#32-extension-methods)
33. [Using / IDisposable](#33-using--idisposable)
34. [Async / Await](#34-async--await)
35. [Collection Expressions](#35-collection-expressions)
36. [Primary Constructors](#36-primary-constructors)
37. [HTTP Client](#37-http-client)
38. [JSON Processing (System.Text.Json)](#38-json-processing-systemtextjson)

---

## 1. Basic Data Types

C# is a **strongly typed** language. Every variable has a type determined at compile time.

### Integral Types

```csharp
byte   b  = 255;            // 8-bit  unsigned  (0 .. 255)
sbyte  sb = -128;           // 8-bit  signed    (-128 .. 127)
short  s  = -32_768;        // 16-bit signed
ushort us = 65_535;          // 16-bit unsigned
int    i  = 42;             // 32-bit signed    (most common)
uint   ui = 42u;            // 32-bit unsigned
long   l  = 9_000_000_000L; // 64-bit signed
ulong  ul = 18_000_000_000UL; // 64-bit unsigned
```

### Floating-Point & Decimal

```csharp
float   f = 3.14f;          // 32-bit, ~6-9 digits precision
double  d = 3.14159265;     // 64-bit, ~15-17 digits precision (default)
decimal m = 19.99m;         // 128-bit, 28-29 digits — use for money!
```

### Other Primitives

```csharp
bool   flag = true;          // true or false
char   c    = 'A';           // single Unicode character (UTF-16)
string name = "Hello";       // immutable reference type
object obj  = 42;            // base type of everything
```

### Special Values

```csharp
double nan      = double.NaN;
double posInf   = double.PositiveInfinity;
int    maxInt   = int.MaxValue;   // 2_147_483_647
int    minInt   = int.MinValue;   // -2_147_483_648
string empty    = string.Empty;   // ""
```

### Type Aliases

| C# Alias  | .NET Type          | Size    |
|-----------|--------------------|---------|
| `bool`    | `System.Boolean`   | 1 byte  |
| `int`     | `System.Int32`     | 4 bytes |
| `long`    | `System.Int64`     | 8 bytes |
| `float`   | `System.Single`    | 4 bytes |
| `double`  | `System.Double`    | 8 bytes |
| `decimal` | `System.Decimal`   | 16 bytes|
| `char`    | `System.Char`      | 2 bytes |
| `string`  | `System.String`    | ref     |
| `object`  | `System.Object`    | ref     |

---

## 2. Boxing / Unboxing

**Boxing** wraps a value type in an `object` on the heap.
**Unboxing** extracts the value type back out.

```csharp
int number = 42;

// Boxing — value type → heap-allocated object
object boxed = number;

// Unboxing — object → value type (must cast to exact type)
int unboxed = (int)boxed;

// Invalid unboxing throws InvalidCastException
// long wrong = (long)boxed; // ❌ Runtime error!
long correct = (int)boxed;   // ✅ Unbox first, then widen
```

> ⚠️ Boxing causes **heap allocation** and is a performance concern in hot paths.
> Generics (`List<int>` instead of `ArrayList`) avoid boxing.

---

## 3. Nullable Types

### Nullable Value Types

Value types cannot be `null` by default. Append `?` to allow it.

```csharp
int? age = null;              // Nullable<int>
age = 25;

// Check and access
if (age.HasValue)
    Console.WriteLine(age.Value);

// GetValueOrDefault
int safeAge = age.GetValueOrDefault(0);

// Null-coalescing
int displayAge = age ?? 0;
```

### Nullable Reference Types (C# 8+)

Enable in the project file (default in .NET 10):

```xml
<Nullable>enable</Nullable>
```

```csharp
string  name    = "Alice";  // Non-nullable — compiler warns if assigned null
string? nickname = null;    // Explicitly nullable

// Compiler warning: possible null dereference
// int len = nickname.Length;

// Safe access
int len = nickname?.Length ?? 0;

// Null-forgiving operator (tell compiler "I know it's not null")
int len2 = nickname!.Length; // Suppresses warning — use with caution
```

---

## 4. Variable Declaration

### Explicit Typing

```csharp
int count = 10;
string message = "Hello";
List<string> names = new List<string>();
```

### Implicit Typing with `var`

The compiler infers the type from the right-hand side.

```csharp
var count   = 10;                  // int
var message = "Hello";             // string
var names   = new List<string>();  // List<string>
```

> 💡 `var` is **not** dynamic — the type is still determined at compile time.

### Target-Typed `new` (C# 9+)

```csharp
List<string> names = new();                // Type inferred from left side
Dictionary<string, int> scores = new();
```

### Constants and Read-Only

```csharp
const double Pi = 3.14159;     // Compile-time constant
readonly int maxRetries = 3;   // Runtime constant (set in ctor)
```

---

## 5. Enumerations

```csharp
// Simple enum (underlying type: int, starts at 0)
enum Season
{
    Spring,    // 0
    Summer,    // 1
    Autumn,    // 2
    Winter     // 3
}

// Explicit values
enum HttpStatus
{
    OK = 200,
    NotFound = 404,
    InternalServerError = 500
}

// Flags enum for bitmask operations
[Flags]
enum Permissions
{
    None    = 0,
    Read    = 1,
    Write   = 2,
    Execute = 4,
    All     = Read | Write | Execute
}

// Usage
Season current = Season.Summer;
string name = current.ToString();       // "Summer"
Season parsed = Enum.Parse<Season>("Winter");

Permissions perms = Permissions.Read | Permissions.Write;
bool canWrite = perms.HasFlag(Permissions.Write); // true
```

---

## 6. Namespaces

### Traditional (Block-Scoped)

```csharp
namespace MyApp.Models
{
    public class Product
    {
        public string Name { get; set; } = "";
    }
}
```

### File-Scoped Namespace (C# 10+) — Preferred ✅

Applies to the entire file, reduces nesting by one level.

```csharp
namespace MyApp.Models;

public class Product
{
    public string Name { get; set; } = "";
}
```

### Global Usings (C# 10+)

```csharp
// In a file like GlobalUsings.cs or in .csproj
global using System.Collections.Generic;
global using System.Linq;

// Implicit usings are enabled by default in .NET 10 projects
// <ImplicitUsings>enable</ImplicitUsings>
```

### Using Aliases

```csharp
using Dict = System.Collections.Generic.Dictionary<string, object>;

// C# 12+: any type alias
using Point = (double X, double Y);

Point origin = (0, 0);
```

---

## 7. Properties

### Auto-Properties

```csharp
public class Product
{
    public string Name { get; set; } = "Unknown";  // with default
    public decimal Price { get; set; }
    public string Id { get; } = Guid.NewGuid().ToString(); // get-only
}
```

### Init-Only Properties (C# 9+)

Can only be set during initialization — immutable after construction.

```csharp
public class Order
{
    public int Id { get; init; }
    public string Customer { get; init; } = "";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

var order = new Order { Id = 1, Customer = "Alice" };
// order.Id = 2; // ❌ Compile error — init-only
```

### Computed Properties

```csharp
public class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double Area => Width * Height;  // expression-bodied, read-only
}
```

### Property with Backing Field

```csharp
public class Temperature
{
    private double _celsius;

    public double Celsius
    {
        get => _celsius;
        set
        {
            if (value < -273.15)
                throw new ArgumentOutOfRangeException(nameof(value));
            _celsius = value;
        }
    }

    public double Fahrenheit => _celsius * 9.0 / 5.0 + 32;
}
```

### The `field` Keyword (C# 13)

Access the auto-generated backing field directly inside property accessors — no need to declare a separate field.

```csharp
public class Temperature
{
    public double Celsius
    {
        get;
        set
        {
            if (value < -273.15)
                throw new ArgumentOutOfRangeException(nameof(value));
            field = value; // 'field' refers to the auto-generated backing field
        }
    }

    public double Fahrenheit => Celsius * 9.0 / 5.0 + 32;
}
```

---

## 8. Classes

### Basic Class

```csharp
public class Person
{
    // Fields
    private readonly string _id = Guid.NewGuid().ToString();

    // Properties
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    // Read-only computed property
    public string FullName => $"{FirstName} {LastName}";

    // Constructor
    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    // Parameterless constructor
    public Person() : this("Unknown", "Unknown", 0) { }

    // Methods
    public string Greet() => $"Hi, I'm {FullName}, age {Age}.";

    // Override ToString
    public override string ToString() => FullName;
}

// Usage
var alice = new Person("Alice", "Smith", 30);
Console.WriteLine(alice.Greet());

var unknown = new Person { FirstName = "Bob", LastName = "Jones", Age = 25 };
```

### Object Initializer

```csharp
var product = new Product
{
    Name = "Laptop",
    Price = 999.99m
};
```

---

## 9. Static Members

```csharp
public class MathHelper
{
    // Static field
    public static readonly double Pi = 3.14159265;

    // Static property
    public static int CallCount { get; private set; }

    // Static method
    public static double CircleArea(double radius)
    {
        CallCount++;
        return Pi * radius * radius;
    }
}

// Usage — accessed via type name, not an instance
double area = MathHelper.CircleArea(5);
Console.WriteLine(MathHelper.CallCount); // 1
```

### Static Class

A class that **cannot** be instantiated — only static members allowed.

```csharp
public static class StringExtensions
{
    public static string Capitalize(this string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];
}
```

---

## 10. Access Modifiers

| Modifier              | Access                                                  |
|-----------------------|---------------------------------------------------------|
| `public`              | Anywhere                                                |
| `private`             | Same class/struct only (default for members)            |
| `protected`           | Same class + derived classes                            |
| `internal`            | Same assembly (default for top-level types)             |
| `protected internal`  | Same assembly **or** derived classes                    |
| `private protected`   | Same assembly **and** derived classes                   |
| `file`                | Same source file only (C# 11+)                         |

```csharp
public class Account
{
    private decimal _balance;                   // only this class
    protected string OwnerId { get; set; }      // this + derived
    internal void Log() { }                     // same assembly
    public decimal Balance => _balance;         // everywhere
}

// File-scoped type (C# 11+)
file class InternalHelper
{
    // Only visible within this source file
    public static void DoWork() { }
}
```

---

## 11. Inheritance

```csharp
// Base class
public abstract class Shape
{
    public string Color { get; set; } = "Black";

    // Abstract — must be implemented by derived classes
    public abstract double Area();

    // Virtual — can be overridden (has a default implementation)
    public virtual string Describe() => $"{Color} shape, area = {Area():F2}";
}

// Derived class
public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius) => Radius = radius;

    public override double Area() => Math.PI * Radius * Radius;

    // Override virtual method
    public override string Describe() =>
        $"{Color} circle (r={Radius}), area = {Area():F2}";
}

// Further derived
public class Cylinder : Circle
{
    public double Height { get; set; }

    public Cylinder(double radius, double height) : base(radius)
    {
        Height = height;
    }

    // New computed property (hides base if name collides)
    public double Volume => Area() * Height;
}

// Sealed — prevents further inheritance
public sealed class Square : Shape
{
    public double Side { get; set; }
    public Square(double side) => Side = side;
    public override double Area() => Side * Side;
}
// class SpecialSquare : Square { } // ❌ Compile error — Square is sealed
```

### Calling Base Members

```csharp
public class LoggingCircle : Circle
{
    public LoggingCircle(double radius) : base(radius) { }

    public override string Describe()
    {
        Console.WriteLine("Describe() called");
        return base.Describe(); // call Circle.Describe()
    }
}
```

---

## 12. Interfaces

### Basic Interface

```csharp
public interface IShape
{
    double Area();
    double Perimeter();
    string Color { get; set; }
}

public class Rectangle : IShape
{
    public double Width { get; set; }
    public double Height { get; set; }
    public string Color { get; set; } = "Black";

    public double Area() => Width * Height;
    public double Perimeter() => 2 * (Width + Height);
}
```

### Multiple Interfaces

```csharp
public interface ISerializable
{
    string Serialize();
}

public interface IPrintable
{
    void Print();
}

public class Report : ISerializable, IPrintable
{
    public string Title { get; set; } = "";
    public string Serialize() => $"{{\"title\":\"{Title}\"}}";
    public void Print() => Console.WriteLine(Title);
}
```

### Default Interface Methods (C# 8+)

```csharp
public interface ILogger
{
    void Log(string message);

    // Default implementation — implementing classes get this for free
    void LogError(string message) => Log($"[ERROR] {message}");
    void LogInfo(string message)  => Log($"[INFO] {message}");
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
    // LogError and LogInfo are available via the interface
}
```

### Static Abstract Members (C# 11+)

```csharp
public interface IParsable<TSelf> where TSelf : IParsable<TSelf>
{
    static abstract TSelf Parse(string s);
}

public class Coordinate : IParsable<Coordinate>
{
    public double X { get; init; }
    public double Y { get; init; }

    public static Coordinate Parse(string s)
    {
        var parts = s.Split(',');
        return new Coordinate
        {
            X = double.Parse(parts[0]),
            Y = double.Parse(parts[1])
        };
    }
}
```

---

## 13. Casting

### Implicit & Explicit Casts

```csharp
// Implicit (widening — safe, no data loss)
int i = 42;
long l = i;
double d = i;

// Explicit (narrowing — possible data loss)
double pi = 3.14;
int rounded = (int)pi;  // 3 — truncates

// Checked arithmetic (throws OverflowException)
checked
{
    byte b = (byte)300; // OverflowException
}
```

### `as` Operator (Reference Types Only)

Returns `null` instead of throwing if cast fails.

```csharp
object obj = "Hello";
string? s = obj as string;   // "Hello"
int?    n = obj as int?;     // null (not an int)
```

### `is` Operator

```csharp
object obj = "Hello";

if (obj is string text)
{
    Console.WriteLine(text.ToUpper()); // "HELLO"
}

if (obj is not null)
{
    Console.WriteLine(obj);
}
```

### Pattern Matching Casts (Preferred ✅)

```csharp
object value = 42;

string result = value switch
{
    int n when n > 0    => $"Positive int: {n}",
    int n               => $"Non-positive int: {n}",
    string s            => $"String: {s}",
    null                => "null",
    _                   => $"Other: {value}"
};
```

---

## 14. Structs

### Basic Struct

Structs are **value types** — stored on the stack (when local), copied on assignment.

```csharp
public struct Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y) => (X, Y) = (x, y);

    public double DistanceTo(Point other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public override string ToString() => $"({X}, {Y})";
}
```

### Readonly Struct

All members must be readonly — guarantees immutability.

```csharp
public readonly struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
```

### Record Struct (C# 10+)

Value-type record with value-based equality.

```csharp
public readonly record struct Coordinate(double Latitude, double Longitude);

var a = new Coordinate(48.2082, 16.3738);
var b = new Coordinate(48.2082, 16.3738);
Console.WriteLine(a == b); // true — value equality
```

---

## 15. Value vs Reference Types

| Aspect          | Value Types (`struct`)         | Reference Types (`class`)       |
|-----------------|--------------------------------|---------------------------------|
| Storage         | Stack (inline)                 | Heap (via reference on stack)   |
| Assignment      | Copies the value               | Copies the reference            |
| Default         | `default` (zeroed)             | `null`                          |
| Equality        | Field-by-field (if overridden) | Reference identity              |
| Examples        | `int`, `bool`, `DateTime`      | `string`, `object`, `List<T>`   |
| Inheritance     | Cannot inherit                 | Can inherit                     |

```csharp
// Value type — independent copies
var p1 = new Point(1, 2);
var p2 = p1;       // copy
p2.X = 99;
Console.WriteLine(p1.X); // 1 — p1 unchanged

// Reference type — shared object
var list1 = new List<int> { 1, 2, 3 };
var list2 = list1;  // same reference
list2.Add(4);
Console.WriteLine(list1.Count); // 4 — list1 is affected
```

---

## 16. Delegates and Events

### Delegates

A delegate is a **type-safe function pointer**.

```csharp
// Declare a delegate type
public delegate double MathOperation(double a, double b);

// Methods matching the signature
static double Add(double a, double b) => a + b;
static double Multiply(double a, double b) => a * b;

// Usage
MathOperation op = Add;
Console.WriteLine(op(3, 4)); // 7

op = Multiply;
Console.WriteLine(op(3, 4)); // 12
```

### Built-In Delegate Types

```csharp
// Func<...> — returns a value (last type parameter is return type)
Func<int, int, int> add = (a, b) => a + b;

// Action<...> — returns void
Action<string> print = msg => Console.WriteLine(msg);

// Predicate<T> — returns bool
Predicate<int> isEven = n => n % 2 == 0;
```

### Events

```csharp
public class Button
{
    // Event declaration using EventHandler
    public event EventHandler<string>? Clicked;

    public void Click()
    {
        Console.WriteLine("Button clicked!");
        Clicked?.Invoke(this, "ButtonClick");
    }
}

// Subscribe
var button = new Button();
button.Clicked += (sender, args) => Console.WriteLine($"Event: {args}");
button.Click();
```

---

## 17. Lambda Expressions

```csharp
// Expression lambda
Func<int, int> square = x => x * x;

// Statement lambda
Func<int, int> factorial = n =>
{
    int result = 1;
    for (int i = 2; i <= n; i++) result *= i;
    return result;
};

// Multiple parameters
Func<double, double, double> hypotenuse = (a, b) => Math.Sqrt(a * a + b * b);

// Discard unused parameters
Action<object, EventArgs> handler = (_, _) => Console.WriteLine("Fired!");
```

### Natural Type for Lambdas (C# 10+)

```csharp
// Compiler infers delegate type
var greet = (string name) => $"Hello, {name}!";   // Func<string, string>
var log   = (string msg) => Console.WriteLine(msg); // Action<string>
```

### Lambda Attributes & Return Types (C# 10+)

```csharp
// Explicit return type
var parse = object (string s) => int.Parse(s);

// Attributes on lambdas
var handler = [Obsolete] () => Console.WriteLine("old");
```

---

## 18. Arrays

```csharp
// Declaration and initialization
int[] numbers = [1, 2, 3, 4, 5];              // collection expression (C# 12)
string[] names = new string[3];                // fixed size, default values
double[,] matrix = { { 1, 2 }, { 3, 4 } };    // 2D array
int[][] jagged = [  [1, 2], [3, 4, 5] ];      // array of arrays

// Access
int first = numbers[0];          // 0-indexed
int last  = numbers[^1];         // index from end (C# 8+)

// Slicing (C# 8+) — returns a new array
int[] middle = numbers[1..4];    // [2, 3, 4]
int[] firstTwo = numbers[..2];   // [1, 2]
int[] lastTwo = numbers[^2..];   // [4, 5]

// Useful members
int length = numbers.Length;
Array.Sort(numbers);
Array.Reverse(numbers);
int idx = Array.IndexOf(numbers, 3);

// Iterate
foreach (int n in numbers)
    Console.Write($"{n} ");
```

---

## 19. Collections

### List\<T\>

```csharp
List<string> fruits = ["Apple", "Banana", "Cherry"]; // C# 12

fruits.Add("Date");
fruits.Remove("Banana");
fruits.Insert(0, "Avocado");
bool has = fruits.Contains("Cherry");       // true
int idx = fruits.IndexOf("Cherry");         // 2
fruits.Sort();
int count = fruits.Count;

// Search
string? found = fruits.Find(f => f.StartsWith("Ch"));
List<string> aFruits = fruits.FindAll(f => f.StartsWith("A"));
```

### Dictionary\<TKey, TValue\>

```csharp
Dictionary<string, int> ages = new()
{
    ["Alice"] = 30,
    ["Bob"]   = 25
};

ages["Charlie"] = 35;               // add or update
ages.TryGetValue("Alice", out int aliceAge); // safe lookup

foreach ((string name, int age) in ages)
    Console.WriteLine($"{name}: {age}");

bool exists = ages.ContainsKey("Bob");
ages.Remove("Bob");
```

### HashSet\<T\>

```csharp
HashSet<int> set = [1, 2, 3, 4, 5];

set.Add(6);           // true  (added)
set.Add(3);           // false (duplicate)
set.Remove(1);
bool has = set.Contains(4); // true

// Set operations
HashSet<int> other = [4, 5, 6, 7];
set.UnionWith(other);        // {2, 3, 4, 5, 6, 7}
set.IntersectWith(other);    // {4, 5, 6, 7}
set.ExceptWith(other);       // {}
```

### Queue\<T\> and Stack\<T\>

```csharp
// FIFO
Queue<string> queue = new();
queue.Enqueue("First");
queue.Enqueue("Second");
string next = queue.Dequeue();  // "First"
string peek = queue.Peek();     // "Second"

// LIFO
Stack<string> stack = new();
stack.Push("Bottom");
stack.Push("Top");
string top = stack.Pop();       // "Top"
```

### Immutable Collections

```csharp
using System.Collections.Immutable;

ImmutableList<int> immutable = [1, 2, 3].ToImmutableList();
ImmutableList<int> added = immutable.Add(4); // returns NEW list
// immutable still has [1, 2, 3]
```

---

## 20. LINQ

> LINQ = **L**anguage **IN**tegrated **Q**uery

### Fluent (Method) Syntax — Preferred ✅

```csharp
List<int> numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// Filtering
var evens = numbers.Where(n => n % 2 == 0);

// Projection
var squared = numbers.Select(n => n * n);

// Ordering
var sorted = numbers.OrderByDescending(n => n);

// Aggregation
int sum   = numbers.Sum();
double avg = numbers.Average();
int max   = numbers.Max();
int count = numbers.Count(n => n > 5);

// First / Single / Last
int first = numbers.First();
int firstEven = numbers.First(n => n % 2 == 0);
int? maybeFirst = numbers.FirstOrDefault(n => n > 100); // 0 (default)

// Existence checks
bool anyOver5 = numbers.Any(n => n > 5);         // true
bool allPositive = numbers.All(n => n > 0);      // true

// Take / Skip
var firstThree = numbers.Take(3);     // [1, 2, 3]
var skipThree  = numbers.Skip(3);     // [4, 5, 6, 7, 8, 9, 10]

// Distinct
int[] unique = [1, 1, 2, 2, 3].Distinct().ToArray();

// GroupBy
var grouped = numbers.GroupBy(n => n % 2 == 0 ? "Even" : "Odd");
foreach (var group in grouped)
    Console.WriteLine($"{group.Key}: {string.Join(", ", group)}");

// Chaining
var result = numbers
    .Where(n => n > 3)
    .Select(n => n * 2)
    .OrderBy(n => n)
    .ToList();
```

### Query Syntax

```csharp
var query = from n in numbers
            where n % 2 == 0
            orderby n descending
            select n * 10;
```

### LINQ with Complex Objects

```csharp
record Student(string Name, int Grade, string Subject);

List<Student> students =
[
    new("Alice", 90, "Math"),
    new("Bob", 85, "Math"),
    new("Charlie", 92, "Science"),
    new("Diana", 88, "Science"),
];

// Group by subject, get average grade
var subjectAverages = students
    .GroupBy(s => s.Subject)
    .Select(g => new { Subject = g.Key, AvgGrade = g.Average(s => s.Grade) });

// Join example
record Teacher(string Name, string Subject);
List<Teacher> teachers = [new("Prof. X", "Math"), new("Prof. Y", "Science")];

var joined = students
    .Join(teachers,
          s => s.Subject,
          t => t.Subject,
          (s, t) => new { Student = s.Name, Teacher = t.Name, s.Subject });
```

---

## 21. Generics

### Generic Class

```csharp
public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T? value, string? error) => (Value, Error) = (value, error);

    public static Result<T> Ok(T value) => new(value, null);
    public static Result<T> Fail(string error) => new(default, error);
}

// Usage
var success = Result<int>.Ok(42);
var failure = Result<int>.Fail("Not found");
```

### Generic Method

```csharp
public static T Max<T>(T a, T b) where T : IComparable<T>
    => a.CompareTo(b) >= 0 ? a : b;

int bigger = Max(3, 7);           // 7
string later = Max("apple", "banana"); // "banana"
```

### Generic Constraints

```csharp
// where T : struct            — value type
// where T : class             — reference type
// where T : class?            — nullable reference type
// where T : notnull           — non-nullable
// where T : new()             — has parameterless constructor
// where T : BaseClass         — derives from BaseClass
// where T : IInterface        — implements interface
// where T : unmanaged         — unmanaged type (no references)

public class Repository<T> where T : class, new()
{
    private readonly List<T> _items = [];

    public T Create()
    {
        var item = new T();    // possible because of new() constraint
        _items.Add(item);
        return item;
    }
}
```

### Generic Interface

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task DeleteAsync(int id);
}
```

---

## 22. Exception Handling

```csharp
try
{
    int result = int.Parse("not a number");
}
catch (FormatException ex)
{
    Console.WriteLine($"Parse error: {ex.Message}");
}
catch (Exception ex) when (ex is not OutOfMemoryException)
{
    // Exception filter — catch block only entered if condition is true
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    // Always runs (cleanup)
    Console.WriteLine("Done.");
}
```

### Throwing Exceptions

```csharp
public void SetAge(int age)
{
    if (age < 0)
        throw new ArgumentOutOfRangeException(nameof(age), "Age must be non-negative");
}

// Re-throw preserving stack trace
try { DoWork(); }
catch (Exception)
{
    throw; // ✅ preserves stack trace (not "throw ex;")
}
```

### Custom Exception

```csharp
public class OrderNotFoundException : Exception
{
    public int OrderId { get; }

    public OrderNotFoundException(int orderId)
        : base($"Order {orderId} was not found.")
    {
        OrderId = orderId;
    }
}
```

---

## 23. Expression-bodied Members

Concise syntax using `=>` for members with a single expression.

```csharp
public class Circle(double radius)
{
    // Property
    public double Radius => radius;

    // Method
    public double Area() => Math.PI * radius * radius;

    // Constructor — only works for single-expression bodies
    // (Use primary constructors for concise ctors, see §36)

    // ToString
    public override string ToString() => $"Circle(r={radius})";

    // Indexer
    public double this[int i] => i == 0 ? radius : throw new IndexOutOfRangeException();

    // Finalizer
    ~Circle() => Console.WriteLine("Finalized");
}
```

---

## 24. Null-Conditional and Null-Coalescing Operators

### Null-Conditional `?.` and `?[]`

Short-circuits to `null` if the left side is `null`.

```csharp
string? name = null;
int? length = name?.Length;            // null (no NullReferenceException)
char? first = name?[0];               // null

// Chaining
string? city = person?.Address?.City;

// With method calls
person?.Notify("Hello");

// With arrays/indexers
int? first = numbers?[0];
```

### Null-Coalescing `??` and `??=`

```csharp
// ?? — provide a fallback value
string display = name ?? "Anonymous";

// ??= — assign only if null
List<string>? items = null;
items ??= [];  // creates list only if null
items.Add("Hello");

// Chaining
string result = first ?? second ?? third ?? "default";
```

---

## 25. String Interpolation and Raw String Literals

### String Interpolation

```csharp
string name = "World";
int year = 2025;

// Basic
string greeting = $"Hello, {name}! Year: {year}";

// Expressions
string info = $"In 10 years it will be {year + 10}";

// Formatting
decimal price = 19.99m;
string formatted = $"Price: {price:C2}";         // "Price: $19.99"
string padded    = $"Name: {name,15}";            // right-aligned, 15 chars
string dateStr   = $"Now: {DateTime.Now:yyyy-MM-dd}";

// Verbatim interpolated
string path = $@"C:\Users\{name}\Documents";
```

### Raw String Literals (C# 11+)

Use `"""` (three or more quotes). No escaping needed.

```csharp
// Single-line
string raw = """This has "quotes" and \backslashes\ without escaping.""";

// Multi-line — leading whitespace is trimmed based on closing quote indentation
string json = """
    {
        "name": "Alice",
        "age": 30
    }
    """;

// With interpolation — use extra $ signs to control brace handling
string name = "Alice";
int age = 30;
string jsonInterpolated = $$"""
    {
        "name": "{{name}}",
        "age": {{age}}
    }
    """;
```

---

## 26. Anonymous Types

Compiler-generated immutable types — useful for projections.

```csharp
var person = new { Name = "Alice", Age = 30 };
Console.WriteLine(person.Name);  // "Alice"
// person.Age = 31;              // ❌ Compile error — read-only

// Common in LINQ projections
var names = students
    .Select(s => new { s.Name, NameLength = s.Name.Length })
    .ToList();

// Anonymous types support equality (by value of all properties)
var a = new { X = 1, Y = 2 };
var b = new { X = 1, Y = 2 };
Console.WriteLine(a.Equals(b)); // true
```

> 💡 Prefer **records** or **tuples** when passing data between methods (anonymous types are type-unsafe outside local scope).

---

## 27. Tuples

### ValueTuple (C# 7+)

```csharp
// Named tuple
(string Name, int Age) person = ("Alice", 30);
Console.WriteLine(person.Name);   // "Alice"

// Unnamed tuple
(string, int) data = ("Bob", 25);
Console.WriteLine(data.Item1);    // "Bob"

// Return multiple values
(int Min, int Max) FindMinMax(int[] arr) =>
    (arr.Min(), arr.Max());

var (min, max) = FindMinMax([3, 1, 4, 1, 5]);
Console.WriteLine($"Min={min}, Max={max}");

// Deconstruction
var (name, age) = person;

// Discard unwanted values
var (_, maxOnly) = FindMinMax([1, 2, 3]);
```

### Deconstruction in Custom Types

```csharp
public class Point(double x, double y)
{
    public double X => x;
    public double Y => y;

    public void Deconstruct(out double x, out double y)
    {
        x = this.X;
        y = this.Y;
    }
}

var point = new Point(3, 4);
var (px, py) = point; // uses Deconstruct
```

---

## 28. Pattern Matching

### Type Patterns (C# 7+)

```csharp
object obj = "Hello";

if (obj is string s)
    Console.WriteLine(s.ToUpper());

if (obj is int n and > 0)
    Console.WriteLine($"Positive: {n}");
```

### Switch Expression (C# 8+)

```csharp
string Classify(object obj) => obj switch
{
    int n when n < 0     => "Negative",
    int n                => $"Integer: {n}",
    string s             => $"String: {s}",
    null                 => "Null",
    _                    => "Unknown"
};
```

### Property Patterns (C# 8+)

```csharp
record Order(string Status, decimal Total, string? CouponCode);

string DescribeOrder(Order order) => order switch
{
    { Status: "Cancelled" }                        => "Cancelled",
    { Total: > 1000, CouponCode: not null }        => "Big order with coupon",
    { Total: > 1000 }                              => "Big order",
    { Total: <= 0 }                                => "Invalid order",
    _                                              => "Regular order"
};
```

### Relational & Logical Patterns (C# 9+)

```csharp
string GetDiscount(decimal total) => total switch
{
    > 1000 => "20% off",
    > 500  => "10% off",
    > 100  => "5% off",
    _      => "No discount"
};

// Logical patterns: and, or, not
string Classify(int n) => n switch
{
    > 0 and < 10  => "Single digit positive",
    >= 10 and < 100 => "Double digit",
    < 0            => "Negative",
    _              => "Zero or large"
};
```

### List Patterns (C# 11+)

```csharp
int[] numbers = [1, 2, 3, 4, 5];

string result = numbers switch
{
    []              => "Empty",
    [var single]    => $"Single: {single}",
    [1, 2, ..]      => "Starts with 1, 2",
    [.., 4, 5]      => "Ends with 4, 5",
    [_, _, var mid, ..] => $"Third element: {mid}",
    _               => "Other"
};
```

---

## 29. Records

### Record Class (C# 9+) — Reference Type

Records provide **value-based equality**, `ToString()`, deconstruction, and `with` expressions.

```csharp
// Positional record
public record Person(string FirstName, string LastName, int Age);

var alice = new Person("Alice", "Smith", 30);
var clone = alice with { Age = 31 };  // non-destructive mutation

Console.WriteLine(alice);             // Person { FirstName = Alice, LastName = Smith, Age = 30 }
Console.WriteLine(alice == clone);    // false (different Age)

var (first, last, age) = alice;       // deconstruction
```

### Record with Additional Members

```csharp
public record Product(string Name, decimal Price)
{
    // Computed property
    public string Display => $"{Name}: {Price:C2}";

    // Additional validation in constructor
    public Product
    {
        if (Price < 0) throw new ArgumentException("Price must be non-negative");
    }
}
```

### Record Struct (C# 10+) — Value Type

```csharp
public record struct Point(double X, double Y)
{
    public double Distance => Math.Sqrt(X * X + Y * Y);
}

// Readonly record struct (immutable)
public readonly record struct Temperature(double Celsius)
{
    public double Fahrenheit => Celsius * 9.0 / 5.0 + 32;
}
```

### Record Inheritance

```csharp
public record Animal(string Name, int Legs);
public record Dog(string Name, string Breed) : Animal(Name, 4);

var dog = new Dog("Rex", "Labrador");
Console.WriteLine(dog); // Dog { Name = Rex, Breed = Labrador, Legs = 4 }
```

---

## 30. Top-level Statements

Starting with C# 9 / .NET 5, the `Main` method boilerplate is optional.

### Minimal Program

```csharp
// Program.cs — this IS the entry point
Console.WriteLine("Hello, World!");
```

### With Args and Return Code

```csharp
// Access command-line args
if (args.Length == 0)
{
    Console.WriteLine("No arguments provided.");
    return 1;
}

Console.WriteLine($"Hello, {args[0]}!");
return 0;
```

### ASP.NET Minimal API (Typical Pattern)

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello, World!");

app.Run();
```

> 💡 Top-level statements compile into a hidden `Main` method. Only **one file** per project can use them.

---

## 31. Required Members

### Required Modifier (C# 11+)

Forces callers to set properties during initialization.

```csharp
public class User
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; set; }
}

// ✅ Must provide required members
var user = new User
{
    Username = "alice",
    Email = "alice@example.com"
};

// ❌ Compile error — missing required member
// var bad = new User { Username = "bob" };
```

### With Constructors

```csharp
public class Config
{
    public required string ConnectionString { get; init; }
    public required int MaxRetries { get; init; }

    [SetsRequiredMembers]
    public Config(string connectionString, int maxRetries)
    {
        ConnectionString = connectionString;
        MaxRetries = maxRetries;
    }
}

// Both are valid:
var c1 = new Config("Server=...;", 3);
var c2 = new Config { ConnectionString = "Server=...;", MaxRetries = 3 };
```

---

## 32. Extension Methods

Static methods that appear as instance methods on the extended type.

```csharp
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? s) =>
        string.IsNullOrEmpty(s);

    public static string Truncate(this string s, int maxLength) =>
        s.Length <= maxLength ? s : s[..maxLength] + "…";

    public static string Reverse(this string s) =>
        new(s.Reverse().ToArray());
}

// Usage — call as instance method
string name = "Hello, World!";
bool empty = name.IsNullOrEmpty();        // false
string short_ = name.Truncate(5);         // "Hello…"
```

### Extension Methods on Interfaces

```csharp
public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        foreach (var item in source)
        {
            if (item is not null)
                yield return item;
        }
    }
}

// Works on any IEnumerable<T?>
string?[] names = ["Alice", null, "Bob", null, "Charlie"];
var nonNull = names.WhereNotNull().ToList(); // ["Alice", "Bob", "Charlie"]
```

---

## 33. Using / IDisposable

### Classic `using` Statement

```csharp
using (var reader = new StreamReader("file.txt"))
{
    string content = reader.ReadToEnd();
} // reader.Dispose() called automatically
```

### `using` Declaration (C# 8+) — Preferred ✅

Disposed at end of enclosing scope — less nesting.

```csharp
void ReadFile()
{
    using var reader = new StreamReader("file.txt");
    string content = reader.ReadToEnd();
    Console.WriteLine(content);
} // reader.Dispose() called here
```

### Implementing IDisposable

```csharp
public class DatabaseConnection : IDisposable
{
    private bool _disposed;

    public void Open() => Console.WriteLine("Connection opened");

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine("Connection closed");
            _disposed = true;
        }
    }
}

// Usage
using var db = new DatabaseConnection();
db.Open();
```

### IAsyncDisposable

```csharp
public class AsyncResource : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await Task.Delay(100); // simulate async cleanup
        Console.WriteLine("Async resource disposed");
    }
}

// Usage
await using var resource = new AsyncResource();
```

---

## 34. Async / Await

### Basics

```csharp
// Async method returns Task (void) or Task<T> (with value)
public async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    string result = await client.GetStringAsync(url);
    return result;
}

// Calling async code
string data = await FetchDataAsync("https://api.example.com/data");
```

### Task vs ValueTask

```csharp
// Use ValueTask when result is often available synchronously (cache hits)
public ValueTask<int> GetCachedValueAsync(string key)
{
    if (_cache.TryGetValue(key, out int value))
        return ValueTask.FromResult(value); // no allocation
    return new ValueTask<int>(FetchFromDatabaseAsync(key));
}
```

### Parallel Async Operations

```csharp
// Run independent tasks concurrently
Task<string> task1 = FetchDataAsync("url1");
Task<string> task2 = FetchDataAsync("url2");
Task<string> task3 = FetchDataAsync("url3");

string[] results = await Task.WhenAll(task1, task2, task3);

// First to complete
Task<string> fastest = await Task.WhenAny(task1, task2, task3);
```

### Async Streams (C# 8+)

```csharp
// Produce items asynchronously
public async IAsyncEnumerable<int> GenerateNumbersAsync(int count)
{
    for (int i = 0; i < count; i++)
    {
        await Task.Delay(100);   // simulate async work
        yield return i;
    }
}

// Consume
await foreach (int number in GenerateNumbersAsync(10))
{
    Console.WriteLine(number);
}
```

### Cancellation

```csharp
public async Task<string> FetchWithTimeoutAsync(string url, CancellationToken ct)
{
    using var client = new HttpClient();
    var response = await client.GetAsync(url, ct);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync(ct);
}

// Usage with timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
try
{
    string data = await FetchWithTimeoutAsync("https://api.example.com", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request timed out");
}
```

### Common Patterns

```csharp
// Async Main (or use top-level statements)
public static async Task Main(string[] args)
{
    await DoWorkAsync();
}

// Async with LINQ
var results = await Task.WhenAll(
    urls.Select(url => FetchDataAsync(url))
);

// Async lazy initialization
private readonly Lazy<Task<Config>> _config =
    new(() => LoadConfigAsync());
```

---

## 35. Collection Expressions (C# 12+)

Unified syntax `[...]` for creating collections.

```csharp
// Arrays
int[] numbers = [1, 2, 3, 4, 5];

// Lists
List<string> names = ["Alice", "Bob", "Charlie"];

// Spans
Span<int> span = [10, 20, 30];
ReadOnlySpan<byte> bytes = [0xFF, 0x00, 0xAB];

// Immutable collections
ImmutableArray<int> immutable = [1, 2, 3];

// Empty collection
int[] empty = [];
List<string> emptyList = [];
```

### Spread Operator `..`

```csharp
int[] first = [1, 2, 3];
int[] second = [4, 5, 6];

// Combine collections
int[] combined = [..first, ..second];         // [1, 2, 3, 4, 5, 6]

// Prepend / append
int[] withZero = [0, ..first];               // [0, 1, 2, 3]
int[] withSeven = [..first, 7];              // [1, 2, 3, 7]

// Flatten
int[][] nested = [[1, 2], [3, 4]];
int[] flat = [..nested[0], ..nested[1]];     // [1, 2, 3, 4]
```

### In Method Parameters

```csharp
void PrintAll(IEnumerable<string> items)
{
    foreach (var item in items)
        Console.WriteLine(item);
}

PrintAll(["one", "two", "three"]); // collection expression as argument
```

---

## 36. Primary Constructors (C# 12+)

### On Classes

Parameters are available throughout the class body.

```csharp
public class UserService(IUserRepository repo, ILogger<UserService> logger)
{
    public async Task<User?> GetUserAsync(int id)
    {
        logger.LogInformation("Fetching user {Id}", id);
        return await repo.GetByIdAsync(id);
    }
}

// Registration in DI — constructor parameters are injected automatically
builder.Services.AddScoped<UserService>();
```

### On Structs

```csharp
public struct Color(byte r, byte g, byte b)
{
    public byte R => r;
    public byte G => g;
    public byte B => b;

    public override string ToString() => $"#{r:X2}{g:X2}{b:X2}";
}
```

### Compared to Records

```csharp
// Record — generates properties, equality, ToString, Deconstruct, with
public record Product(string Name, decimal Price);

// Class with primary constructor — does NOT generate properties automatically
// You must define them yourself if needed
public class ProductService(IRepository<Product> repo)
{
    // 'repo' is a parameter, not a property
    public Task<List<Product>> GetAllAsync() => repo.GetAllAsync();
}
```

### Capturing and Validation

```csharp
public class Temperature(double celsius)
{
    // Validate in field initializer or property
    private readonly double _celsius = celsius >= -273.15
        ? celsius
        : throw new ArgumentOutOfRangeException(nameof(celsius));

    public double Celsius => _celsius;
    public double Fahrenheit => _celsius * 9.0 / 5.0 + 32;
}
```

---

## 37. HTTP Client

### Basic Usage

```csharp
using var client = new HttpClient
{
    BaseAddress = new Uri("https://api.example.com/")
};

// GET
string json = await client.GetStringAsync("products");

// GET with deserialization
var products = await client.GetFromJsonAsync<List<Product>>("products");

// POST
var newProduct = new Product("Widget", 9.99m);
var response = await client.PostAsJsonAsync("products", newProduct);
response.EnsureSuccessStatusCode();
var created = await response.Content.ReadFromJsonAsync<Product>();
```

### IHttpClientFactory (Recommended for ASP.NET)

```csharp
// Registration in Program.cs
builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Typed client
public class ProductApiClient(HttpClient client)
{
    public async Task<List<Product>> GetAllAsync()
        => await client.GetFromJsonAsync<List<Product>>("products") ?? [];

    public async Task<Product?> GetByIdAsync(int id)
        => await client.GetFromJsonAsync<Product>($"products/{id}");

    public async Task<Product?> CreateAsync(Product product)
    {
        var response = await client.PostAsJsonAsync("products", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task UpdateAsync(int id, Product product)
    {
        var response = await client.PutAsJsonAsync($"products/{id}", product);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await client.DeleteAsync($"products/{id}");
        response.EnsureSuccessStatusCode();
    }
}
```

### Request Customization

```csharp
// Custom request with headers
var request = new HttpRequestMessage(HttpMethod.Get, "products");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
request.Headers.Add("X-Custom-Header", "value");

var response = await client.SendAsync(request);
var content = await response.Content.ReadAsStringAsync();

// Check status
if (response.IsSuccessStatusCode)
{
    var data = await response.Content.ReadFromJsonAsync<Product>();
}
else
{
    Console.WriteLine($"Error: {response.StatusCode}");
}
```

---

## 38. JSON Processing (System.Text.Json)

### Serialization / Deserialization

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

var product = new Product("Widget", 9.99m);

// Serialize to JSON string
string json = JsonSerializer.Serialize(product);
// {"Name":"Widget","Price":9.99}

// Serialize with options
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};
string prettyJson = JsonSerializer.Serialize(product, options);
// {
//   "name": "Widget",
//   "price": 9.99
// }

// Deserialize
var deserialized = JsonSerializer.Deserialize<Product>(json);
```

### Attributes for Customization

```csharp
public class User
{
    [JsonPropertyName("user_name")]
    public string Username { get; set; } = "";

    [JsonPropertyName("email_address")]
    public string Email { get; set; } = "";

    [JsonIgnore]
    public string PasswordHash { get; set; } = "";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public enum UserRole { Admin, User, Guest }
```

### Source Generators (AOT-Friendly)

```csharp
// Declare a serializer context — enables AOT compilation
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(List<Product>))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
public partial class AppJsonContext : JsonSerializerContext { }

// Usage
string json = JsonSerializer.Serialize(product, AppJsonContext.Default.Product);
var parsed = JsonSerializer.Deserialize(json, AppJsonContext.Default.Product);
```

### Working with Dynamic JSON (JsonDocument / JsonNode)

```csharp
// JsonDocument — read-only, fast
using var doc = JsonDocument.Parse(json);
JsonElement root = doc.RootElement;
string name = root.GetProperty("name").GetString()!;
decimal price = root.GetProperty("price").GetDecimal();

// JsonNode — mutable DOM (C# 10+)
var node = JsonNode.Parse(json)!;
string? userName = node["name"]?.GetValue<string>();

// Build JSON dynamically
var obj = new JsonObject
{
    ["name"] = "New Product",
    ["price"] = 29.99,
    ["tags"] = new JsonArray("electronics", "sale"),
    ["metadata"] = new JsonObject
    {
        ["created"] = DateTime.UtcNow.ToString("o"),
        ["version"] = 2
    }
};
string dynamicJson = obj.ToJsonString(options);
```

### JSON in ASP.NET Minimal API

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure JSON options globally
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapGet("/products", () => new List<Product>
{
    new("Widget", 9.99m),
    new("Gadget", 19.99m)
});

app.MapPost("/products", (Product product) =>
{
    // product is automatically deserialized from request body
    return Results.Created($"/products/{product.Name}", product);
});

app.Run();
```

---

## Quick Reference: What's New by Version

| Version | Key Features |
|---------|-------------|
| **C# 8** | Nullable reference types, `using` declarations, default interface methods, switch expressions, async streams, index/range operators |
| **C# 9** | Records, top-level statements, init-only properties, pattern matching enhancements, target-typed `new` |
| **C# 10** | File-scoped namespaces, global usings, record structs, `const` interpolated strings, natural type for lambdas |
| **C# 11** | Raw string literals, required members, list patterns, `file` access modifier, static abstract interface members, generic attributes |
| **C# 12** | Collection expressions, primary constructors, alias any type, inline arrays, interceptors (preview) |
| **C# 13** | `field` keyword in properties, `params` collections, `Lock` type, `\e` escape sequence, method group natural type improvements |

---

*Happy coding! 🚀*
