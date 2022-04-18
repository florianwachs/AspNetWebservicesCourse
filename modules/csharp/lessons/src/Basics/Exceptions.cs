using Xunit;

namespace Basics;

public class Exceptions
{
    [Fact]
    public void Demo1()
    {
        Assert.Throws<NotImplementedException>(() =>
        {
            try
            {
                NotDoneYet();
            }
            catch (InvalidOperationException)
            {
                // Fängt nur Exceptions von diesem Typ
            }
            catch (Exception ex)
            {
                // Fängt alle restlichen Exceptions

                // wirft die Exception weiter den Stacktrace rauf
                throw;

                // wirft die Exception weiter den Stackrace rauf, der Stacktrace beginnt aber erst von dieser Stelle
                //throw ex;
            }
            finally
            {
                // Wird auf "jeden" Fall ausgeführt
                // Aufräumen, z.B. File-Handles schließen
            }
        });
    }

    private void NotDoneYet()
    {
        throw new NotImplementedException("Hab´s doch gesagt!");
    }
}
