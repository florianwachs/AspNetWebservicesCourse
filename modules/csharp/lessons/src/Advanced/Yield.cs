using Xunit;

namespace Advanced;

public class Yield
{
    [Fact]
    public void DemoMitYield()
    {
        // Obwohl wir max. 10000 Randoms
        // generieren wolle, wird die Generierung
        // nach 5 beendet
        var query = GetRandom(10000).Take(5);

        var i = 1;
        foreach (var item in query)
        {
            Console.WriteLine("{0}: {1}", i++, item);
        }
    }

    [Fact]
    public void DemoOhneYield()
    {
        var query = new RandomEnumerator(10000).Take(5);
        var i = 1;
        foreach (var item in query)
        {
            Console.WriteLine("{0}: {1}", i++, item);
        }
    }

    // die Verwendung von yield erspart die Definition
    // einer eigenen Klasse die IEnumerator und IEnumerable implementiert
    public static IEnumerable<int> GetRandom(int count)
    {
        var rnd = new Random();
        var hardBreak = 100;
        for (int i = 0; i < count; i++)
        {
            if (i == hardBreak)
            {
                yield break;
            }
            // yield return gibt die Kontrolle an
            // den Aufrufer zurück. Erst wenn der Aufrufer
            // wieder weiter iteriert, wird hier weiter gemacht
            yield return rnd.Next();
        }
    }

    // analog zum DemoMitYield: Das müsste ohne yield implementiert werden
    // um das gleiche Verhalten zu erhalten
    public class RandomEnumerator : IEnumerator<int>, IEnumerable<int>
    {
        private readonly int hardBreak = 100;
        private readonly int count;
        private readonly Random rnd;

        private int i;
        private int current;

        public RandomEnumerator(int count)
        {
            this.count = count;
            rnd = new Random();
        }

        public int Current
        {
            get
            {
                return current;
            }
        }

        public bool MoveNext()
        {
            var currentIndex = i++;
            var hasNext = currentIndex < count && currentIndex != hardBreak;
            if (hasNext)
            {
                current = rnd.Next();
            }

            return hasNext;
        }

        object System.Collections.IEnumerator.Current
        {
            get { throw new NotImplementedException(); }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
