using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Security.Client.Dtos;

namespace WebAPI.Security.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        public static async Task RunAsync()
        {
            string uri = "http://localhost:4862/";
            var client = new HttpClient();
            client.BaseAddress = new Uri(uri, UriKind.Absolute);

            do
            {
                try
                {
                    Console.WriteLine("Enter Method: (login/logout/get/getall/insert/update/delete)");
                    string method = Console.ReadLine();

                    // einloggen
                    if (method.Equals("login"))
                    {
                        Console.WriteLine("Enter username: ");
                        var username = Console.ReadLine();
                        Console.WriteLine("Enter password: ");
                        var password = Console.ReadLine();

                        var formValues = new Dictionary<string, string>
                        {
                            {"grant_type","password"},
                            {"username", username},
                            {"password", password}
                        };

                        FormUrlEncodedContent loginContent = new FormUrlEncodedContent(formValues);
                        var response = await client.PostAsync("token", loginContent);
                        response.EnsureSuccessStatusCode();
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_Token);
                        Console.WriteLine("Eingeloggt als " + tokenResponse.Username);
                    }

                    if (method.Equals("logout"))
                    {
                        client.DefaultRequestHeaders.Authorization = null;
                        Console.WriteLine("logout");
                    }

                    // einzelne Person holen 
                    if (method.Equals("get"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();

                        var response = await client.GetAsync("api/personen/" + id);
                        response.EnsureSuccessStatusCode();
                        var person = await response.Content.ReadAsAsync<Person>();
                        LogToConsole(person);
                    }
                    // alle Personen holen 
                    else if (method.Equals("getall"))
                    {
                        var response = await client.GetAsync("api/personen");
                        response.EnsureSuccessStatusCode();
                        var personen = await response.Content.ReadAsAsync<Person[]>();

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

                        Person p = new Person { Name = name, Age = age, Id = "0" };
                        var response = await client.PostAsJsonAsync("api/personen", p);
                        response.EnsureSuccessStatusCode();
                        var newPerson = await response.Content.ReadAsAsync<Person>();
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

                        Person p = new Person { Name = name, Age = age, Id = "0" };
                        var response = await client.PutAsJsonAsync("api/personen/" + id, p);
                        response.EnsureSuccessStatusCode();
                        var updatedPerson = await response.Content.ReadAsAsync<Person>();
                        LogToConsole(updatedPerson);

                    }
                    // Person löschen mit DELETE
                    else if (method.Equals("delete"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();
                        var response = await client.DeleteAsync("api/personen/" + id);
                        response.EnsureSuccessStatusCode();
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
    }
}
