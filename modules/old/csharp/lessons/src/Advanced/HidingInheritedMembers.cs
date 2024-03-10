namespace Advanced;

// new für member hiding ist nur
// in extrem seltenen Ausnahmefällen sinnvoll.
// Meist, wenn die Funktionalität die überschrieben
// werden soll, in einer externen 
// Abhängigkeit (3rd-party-Framework) liegt die nicht
// verändert werden kann.
public static class HidingInheritedMembers
{
    public static void Demo1()
    {
        var p = new CompatPerson() { FirstName = "Hans", LastName = "Meiser" };
        Console.WriteLine(p.IsNew);
        p.Save();
    }
}

public class CompatPerson : Person
{
    // ohne new würde ein compile-Fehler auftreten
    // da IsNew bereits definiert ist.
    public new bool IsNew
    {
        get { return false; }
    }

    // Auch Methoden der Basis können mit new versteckt werden
    public new void Save()
    {
        Console.WriteLine("Save");
        base.Save();
    }
}
