using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class OptionalNamedParameters
    {
        public static void Demo1()
        {
            // nur der erforderliche Parameter wurde gesetzt
            // für alle anderen wird der Standard verwendet
            WriteToConsole("Hi there");

            // alle Parameter können auch einfach angegeben werden
            WriteToConsole("Alarmstufe ROT", ConsoleColor.Red, false);

            // es können beliebige optionale Parameter als named Parameter gefüllt werden
            WriteToConsole("What´s the time? Oh there it is...", useTimeStamp: true);

        }

        public static void WriteToConsole(string msg, ConsoleColor forgroundColor = ConsoleColor.Blue, bool useTimeStamp = false)
        {
            Console.ForegroundColor = forgroundColor;
            var sb = new StringBuilder();
            if (useTimeStamp)
            {
                sb.AppendFormat("[{0}]",DateTime.Now.ToString());
            }
            sb.Append(msg);

            Console.WriteLine(sb.ToString());
            Console.ResetColor();
        }
    }
}
