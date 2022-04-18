using Xunit;

namespace Basics;

public class PassingByValueVsPassingByReference
{
    [Fact]
    public void PassingByValue()
    {
        var student = new Student { FirstName = "Liese", LastName = "Müller" };
        Console.WriteLine("Student vor Foo(): " + student.ToString());
        FooByValue(student);
        Console.WriteLine("Student nach Foo(): " + student.ToString());
    }

    [Fact]
    public void PassingByReference()
    {
        var student = new Student { FirstName = "Liese", LastName = "Müller" };
        Console.WriteLine("Student vor Foo(): " + student.ToString());
        FooByReference(ref student);
        Console.WriteLine("Student nach Foo(): " + student.ToString());
    }

    [Fact]
    public void OutDemo1()
    {
        var test = "Hans Meiser";
        string[] names;
        if (SplitNames(test, out names))
        {
            // foo
        }
    }

    private void FooByValue(Student s)
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

    private void FooByReference(ref Student s)
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

    private bool SplitNames(string text, out string[] names)
    {
        // Muss vor dem Verlassen der Methode zugewiesen werden
        names = null;

        if (!string.IsNullOrWhiteSpace(text))
        {
            names = text.Split();
        }

        return names != null && names.Length > 0;
    }
}
