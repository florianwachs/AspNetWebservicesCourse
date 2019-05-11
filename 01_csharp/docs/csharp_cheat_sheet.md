# CSharp Cheat Sheet

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
