using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCF.Soap.Client.UniversityService;

namespace WCF.Soap.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSync();
            RunAsync().Wait();

            Console.WriteLine("[key]");
            Console.ReadKey();
        }

        private static void PrintStudents(Student[] students)
        {
            Console.WriteLine(new string('*', 10));
            foreach (var student in students)
            {
                Console.WriteLine("[{0}] {1} {2}", student.Id, student.FirstName, student.LastName);
            }
            Console.WriteLine(new string('*', 10));
            Console.WriteLine();
        }

        private static async Task RunAsync()
        {
            using (var client = new UniversityServiceClient())
            {
                var students = await client.GetStudentsAsync();
                // Studenten über den Service abrufen
                PrintStudents(students);

                // Neuen Studenten hinzufügen
                Console.WriteLine("Füge neuen Studenten hinzu und update bestehenden");
                var chan = await client.AddStudentAsync(new Student { FirstName = "Jackie", LastName = "Chan" });

                var s = students[0];
                s.LastName = "Müller";

                await client.UpdateStudentAsync(s);

                PrintStudents(await client.GetStudentsAsync());

                Console.WriteLine("Lösche neuen Studenten");
                await client.DeleteStudentAsync(chan.Id);

                PrintStudents(await client.GetStudentsAsync());
            }
        }

        private static void RunSync()
        {
            using (var client = new UniversityServiceClient())
            {
                var students = client.GetStudents();
                // Studenten über den Service abrufen
                PrintStudents(students);

                // Neuen Studenten hinzufügen
                Console.WriteLine("Füge neuen Studenten hinzu und update bestehenden");
                var chan = client.AddStudent(new Student { FirstName = "Jackie", LastName = "Chan" });

                var s = students[0];
                s.LastName = "Müller";

                client.UpdateStudent(s);

                PrintStudents(client.GetStudents());

                Console.WriteLine("Lösche neuen Studenten");
                client.DeleteStudent(chan.Id);

                PrintStudents(client.GetStudents());
            }
        }
    }
}
