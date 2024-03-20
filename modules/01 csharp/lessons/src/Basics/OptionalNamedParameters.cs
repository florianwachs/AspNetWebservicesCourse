using System.Text;
using Xunit;

namespace Basics;

public class OptionalNamedParameters
{
    [Fact]
    public void Demo1()
    {
        // nur der erforderliche Parameter wurde gesetzt
        // für alle anderen wird der Standard verwendet
        WriteToConsole("Hi there");

        // alle Parameter können auch einfach angegeben werden
        WriteToConsole("Alarmstufe ROT", ConsoleColor.Red, false);

        // es können beliebige optionale Parameter als named Parameter gefüllt werden
        WriteToConsole("What´s the time? Oh there it is...", useTimeStamp: true);

    }

    public void WriteToConsole(string msg, ConsoleColor forgroundColor = ConsoleColor.Blue, bool useTimeStamp = false)
    {
        Console.ForegroundColor = forgroundColor;
        var sb = new StringBuilder();
        if (useTimeStamp)
        {
            sb.AppendFormat("[{0}]", DateTime.Now.ToString());
        }
        sb.Append(msg);

        Console.WriteLine(sb.ToString());
        Console.ResetColor();
    }
}
