using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class Exceptions
    {
        public static void NotDoneYet()
        {
            throw new NotImplementedException("Hab´s doch gesagt!");
        }

        public static void Demo1()
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
                throw ex;
            }
            finally
            {
                // Wird auf "jeden" Fall ausgeführt
                // Aufräumen, z.B. File-Handles schließen
            }
        }
    }
}
