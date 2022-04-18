using Xunit;

namespace Basics;

public class Casting
{
    [Fact]
    public void BasicTypes()
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

    [Fact]
    public void ReferenceTypes()
    {
        // Student : Person
        var student = new Student();

        // implicit upcast
        Person p = student;

        // wirft einen Fehler
        // student = p;      

        student = (Student)p;
    }

    [Fact]
    public void AsIs()
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
}
