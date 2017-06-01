using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Security.OpenIddict.Client.Models;

namespace AspNetCore.Security.OpenIddict.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        public static async Task RunAsync()
        {
            string uri = "http://localhost:24677/";
            var client = new HttpClient();
            client.BaseAddress = new Uri(uri, UriKind.Absolute);

            do
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("Enter Method: (login/logout/get/getall/insert/update/delete/exit)");
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
                            {"password", password},
                            {"scope","openid+email+name+profile+roles+offline_access" }
                        };

                        var loginContent = new FormUrlEncodedContent(formValues);
                        var response = await client.PostAsync("connect/token", loginContent);
                        response.EnsureSuccessStatusCode();
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_Token);
                        Console.WriteLine($"Eingeloggt als {username}");
                    }

                    if (method.Equals("exit"))
                    {
                        Environment.Exit(0);
                    }

                    if (method.Equals("logout"))
                    {
                        client.DefaultRequestHeaders.Authorization = null;
                        Console.WriteLine("logout");
                    }

                    // einzelne Customer holen 
                    if (method.Equals("get"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();

                        var response = await client.GetAsync("api/customers/" + id);
                        response.EnsureSuccessStatusCode();
                        var person = await response.Content.ReadAsAsync<Customer>();
                        LogToConsole(person);
                    }
                    // alle Customeren holen 
                    else if (method.Equals("getall"))
                    {
                        var response = await client.GetAsync("api/customers");
                        response.EnsureSuccessStatusCode();
                        var customers = await response.Content.ReadAsAsync<Customer[]>();

                        foreach (var person in customers)
                        {
                            LogToConsole(person);
                        }
                    }
                    // neue Customer erzeugen
                    else if (method.Equals("insert"))
                    {
                        Console.WriteLine("Enter Name: ");
                        var name = Console.ReadLine();
                        Console.WriteLine("Enter Alter: ");
                        var age = Console.ReadLine();

                        Customer p = new Customer { Name = name, Age = age, Id = "0" };
                        var response = await client.PostAsJsonAsync("api/customers", p);
                        response.EnsureSuccessStatusCode();
                        var newCustomer = await response.Content.ReadAsAsync<Customer>();
                        LogToConsole(newCustomer);
                    }
                    // Customer ändern
                    else if (method.Equals("update"))
                    {
                        Console.WriteLine("Enter id: ");
                        var id = Console.ReadLine();
                        Console.WriteLine("Enter Name: ");
                        var name = Console.ReadLine();
                        Console.WriteLine("Enter Alter: ");
                        var age = Console.ReadLine();

                        Customer p = new Customer { Name = name, Age = age, Id = "0" };
                        var response = await client.PutAsJsonAsync("api/customers/" + id, p);
                        response.EnsureSuccessStatusCode();
                        var updatedCustomer = await response.Content.ReadAsAsync<Customer>();
                        LogToConsole(updatedCustomer);

                    }
                    // Customer löschen mit DELETE
                    else if (method.Equals("delete"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();
                        var response = await client.DeleteAsync("api/customers/" + id);
                        response.EnsureSuccessStatusCode();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine();
                Console.WriteLine("Weiter mit dem [Any] Key, aber NUR DEM!");
                Console.ReadKey();

            } while (true);
        }

        private static void LogToConsole(Customer p)
        {
            if (p == null)
            {
                Console.WriteLine("Customer ist null");
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