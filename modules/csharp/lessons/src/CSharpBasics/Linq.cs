using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class Linq
    {
        public static void MethodSyntax()
        {
            Console.WriteLine();

            var test = Enumerable.Range(0, 50);

            var methodQuery = test
                .Where(number => number % 2 == 0 && number > 10)                
                .Select(number => Math.Pow(number, 2));

            // methodQuery wird erst ausgewertet, wenn iteriert wird
            foreach (var number in methodQuery)
            {
                Console.Write(number + " ");
            }
        }

        public static void QuerySyntax()
        {
            Console.WriteLine();

            var test = Enumerable.Range(0, 50);

            var query = from number in test
                        where (number % 2) == 0
                        select Math.Pow(number, 2);

            // query wird erst ausgewertet, wenn iteriert wird
            foreach (var number in query)
            {
                Console.Write(number + " ");
            }
        }

        public static void Demo1()
        {
            IStudentRepository repo = new StudentsDuringPartyRepository();

            foreach (var student in repo.GetAll()
                .Where(student =>
                    student.Motivation > 5.0m && student.FirstName.StartsWith("A"))
                    .OrderBy(student => student.FirstName))
            {
                // Foo
            }

        }
    }
}
