using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreUnitTests.UnitTests.Containers
{
    public class EuContainerTests : IDisposable
    {
        public EuContainerTests()
        {
            // Im Kontruktor können vom Test benötigte Objekte instanziert werden.
            // Für jeden Test wird der Konstruktor erneut ausgeführt.
        }

        // Fact über einer Methode informiert xUnit, das dies eine Testmethode ist.
        [Fact]
        public void ASimpleTest()
        {

        }

        // Testmethoden können auch Asyncron sein.
        public async Task AAsyncTest()
        {
        }


        // Mittels Theory kann die selbe Testspezifikation mehrfach mit verschiedenen
        // Daten ausgeführt werden. Es gibt auch die Möglichkeit diese Daten aus
        // einer anderen Klasse oder Datei zu laden.
        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(10, 5, 5)]
        [InlineData(4, 1, 3)]
        public void OneDefinitionMultipleTestData(int expected, int a, int b)
        {

        }

        public void Dispose()
        {
            // Optional:
            // Werden im Konstruktor Ressourcen erzeugt, welche Disposed werden müssen
            // (File Handles, Netzwerk,...), kann die Testklasse IDisposable implementieren
            // xUnit ruft Dispose automatisch auf.
        }
    }
}
