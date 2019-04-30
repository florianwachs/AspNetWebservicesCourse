using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Client.Domain;

namespace WebAPI.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        public static async Task RunAsync()
        {
            string uri = "http://localhost:11894/api/";
            var client = new HttpClient();
            client.BaseAddress = new Uri(uri, UriKind.Absolute);

            do
            {
                try
                {
                    Console.WriteLine("Enter Method: (get/getall/insert/update/delete)");
                    string method = Console.ReadLine();

                    // einzelne Person holen 
                    if (method.Equals("get"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();


                        var person = await JsonRequestAsync<Person>(client, HttpMethod.Get, "personen/" + id);
                        LogToConsole(person);

                    }
                    // alle Personen holen 
                    else if (method.Equals("getall"))
                    {
                        var personen = await JsonRequestAsync<Person[]>(client, HttpMethod.Get, "personen");

                        foreach (var person in personen)
                        {
                            LogToConsole(person);
                        }
                    }
                    // neue Person erzeugen
                    else if (method.Equals("insert"))
                    {
                        Console.WriteLine("Enter Name: ");
                        var name = Console.ReadLine();
                        Console.WriteLine("Enter Alter: ");
                        var age = Console.ReadLine();

                        Person p = new Person { Name = name, Age = age, Id = 0 };

                        var newPerson = await JsonRequestAsync<Person>(client, HttpMethod.Post, "personen", p);
                        LogToConsole(newPerson);
                    }
                    // Person ändern
                    else if (method.Equals("update"))
                    {
                        Console.WriteLine("Enter id: ");
                        var id = Console.ReadLine();
                        Console.WriteLine("Enter Name: ");
                        var name = Console.ReadLine();
                        Console.WriteLine("Enter Alter: ");
                        var age = Console.ReadLine();

                        Person p = new Person { Name = name, Age = age, Id = 0 };

                        var updatedPerson = await JsonRequestAsync<Person>(client, HttpMethod.Put, "personen/" + id, p);
                        LogToConsole(updatedPerson);

                    }
                    // Person löschen mit DELETE
                    else if (method.Equals("delete"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();

                        await JsonRequestAsync<object>(client, HttpMethod.Delete, "personen/" + id);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }

                Console.WriteLine();
                Console.WriteLine("Do you want to continue? (Y)");
            } while (Console.ReadLine().ToUpper() == "Y");
        }

        private static void LogToConsole(Person p)
        {
            if (p == null)
            {
                Console.WriteLine("Person ist null");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Name: " + p.Name);
            sb.AppendLine("Age: " + p.Age);
            sb.AppendLine("Id: " + p.Id);

            Console.WriteLine(sb.ToString());
        }

        private static async Task<T> JsonRequestAsync<T>(HttpClient client, HttpMethod method, string uri, object payload = null)
        {
            HttpRequestMessage req = new HttpRequestMessage(method, uri);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;
            if (payload != null)
            {
                var ser = JsonConvert.SerializeObject(payload);
                req.Content = new StringContent(ser);
                req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.SendAsync(req);
            }
            else
            {
                response = await client.SendAsync(req);
            }

            // Wirft einen Fehler wenn der Serviceaufruf einen Fehler am Server generierte
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength.GetValueOrDefault(0) > 0)
            {
                var jObj = (JContainer)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                return (T)jObj.ToObject(typeof(T));
            }

            return default(T);
        }
    }
}
