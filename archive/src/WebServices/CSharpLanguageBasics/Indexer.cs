using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class Indexer
    {
        public static void Demo1()
        {
            // auch ein String hat einen Indexer
            var msg = "Hello World";
            Console.WriteLine(msg[4]);
        }

        public static void Demo2()
        {
            var t = new Sentence("The quick brown fox");
            Console.WriteLine(t[2]); // quick
            t[2] = "old";
            Console.WriteLine(t.ToString());
        }
    }

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
}
