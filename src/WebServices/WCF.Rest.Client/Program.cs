using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using WCF.Rest.Client.Dtos;

namespace WCF.Rest.Client
{
    class Program
    {
        static void Main(string[] args)
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

        // ACHTUNG:
        // da wir async und await noch nicht durchgenommen haben
        // muss hier an den Task-basierten Methoden mit Result oder Wait() gearbeitet werden
        // um auf die Beendigung des Requests zu warten
        public class UniversityServiceClient : IDisposable
        {
            private readonly HttpClient client;
            private Uri baseUri = new Uri("http://localhost:51553/UniversityService.svc/", UriKind.Absolute);

            public UniversityServiceClient()
            {
                // Der HTTP-Client ist für die Mehrfachverwendung ausgelegt,
                // auch bei mehreren Threads
                client = new HttpClient();
                // optional: wird die BaseAddress gesetzt können abfragen relativ zu ihr gemacht werden
                client.BaseAddress = baseUri;
                // optional: ist klar das nur JSON abgefragt wird, kann dies standardmäßig im Header mitgeteilt werden.
                // Allerdings muss der Service dies auch auswerten
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            public Student[] GetStudents()
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Student[]));

                // Mit dem HttpClient können nicht nur strings sondern gleich der
                // ankommende Netzwerkstream verarbeitet werden. Das ist nützlich, da
                // der Serializer ein Streamobjekt verarbeiten kann
                using (var dataStream = client.GetStreamAsync("students").Result)
                {
                    return (Student[])ser.ReadObject(dataStream);
                }
            }

            public Student GetStudentById(int id)
            {
                // Über HttpRequestMessage können alle Details eines Requests spezifiziert werden
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, "students/" + id);

                // Es ist best-practice, immer mitzuteilen welches Format man als Ergebnis möchte
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

                // Request abschicken und mit Result auf Beendigung warten
                var response = client.SendAsync(req).Result;

                // Wirft einen Fehler wenn der Serviceaufruf einen Fehler am Server generierte
                response.EnsureSuccessStatusCode();

                // Kümmert sich um die XML Deserialisierung
                // Das DTO muss zu dem am Server serialisierten Daten passen (Namespace)
                DataContractSerializer ser = new DataContractSerializer(typeof(Student));
                var obj = ser.ReadObject(response.Content.ReadAsStreamAsync().Result);
                return (Student)obj;
            }

            public Student AddStudent(Student s)
            {
                // JSON.NET ist eine hochperformante Library die auch in anderen 
                // Microsoft Produkten wie Web API einsatz findet
                var serialized = JsonConvert.SerializeObject(s);

                // Daten müssen für den Transport verpackt werden
                var payload = new StringContent(serialized);
                payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync("students", payload).Result;
                response.EnsureSuccessStatusCode();

                // Das Ergebnis kann mit JsonConvert wieder zurückserialisiert werden
                var addedStudent = JsonConvert.DeserializeObject<Student>(response.Content.ReadAsStringAsync().Result);
                return addedStudent;
            }

            public void UpdateStudent(Student s)
            {
                var serialized = JsonConvert.SerializeObject(s);
                var payload = new StringContent(serialized);
                payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.PutAsync("students/" + s.Id, payload).Result;
            }

            public void DeleteStudent(int id)
            {
                var response = client.DeleteAsync("students/" + id).Result;
            }

            public void Dispose()
            {
                client.Dispose();
            }

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
    }
}
