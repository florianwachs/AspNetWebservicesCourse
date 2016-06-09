using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WCF.Rest.Client.Dtos;

namespace WCF.Rest.Client
{
    public class ProgramXml
    {      

        public static async Task RunAsync()
        {
            string uri = "http://localhost:TODO/TODO/";
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


                        // TODO Webservice-Aufruf


                        // TODO Ergebnis auf Konsole ausgeben

                    }
                    // alle Personen holen 
                    else if (method.Equals("getall"))
                    {


                        // TODO Webservice-Aufruf


                        // TODO Ergebnis auf Konsole ausgeben

                    }
                    // neue Person erzeugen
                    else if (method.Equals("insert"))
                    {
                        Console.WriteLine("Enter Name: ");
                        var name = Console.ReadLine();
                        Console.WriteLine("Enter Alter: ");
                        var age = Console.ReadLine();

                        Person p = new Person { Name = name, Age = age, Id = "0" };

                        // TODO Webservice-Aufruf


                        // TODO Ergebnis auf Konsole ausgeben

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

                        // TODO Webservice-Aufruf


                        // TODO Ergebnis auf Konsole ausgeben


                    }
                    // Person löschen mit DELETE
                    else if (method.Equals("delete"))
                    {
                        Console.WriteLine("Enter id: ");
                        string id = Console.ReadLine();

                        // TODO Webservice-Aufruf



                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }

                Console.WriteLine();
                Console.WriteLine("Do you want to continue?");
            } while (Console.ReadLine().ToUpper() == "Y");
        }

        #region Helper
        private static async Task<T> XmlRequestAsync<T>(HttpClient client, HttpMethod method, string uri, object payload = null)
        {
            HttpRequestMessage req = new HttpRequestMessage(method, uri);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            HttpResponseMessage response = null;
            if (payload != null)
            {
                var ser = new DataContractSerializer(payload.GetType());
                using (var stream = new MemoryStream())
                {
                    ser.WriteObject(stream, payload);
                    stream.Position = 0;
                    req.Content = new StreamContent(stream);
                    req.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                    response = await client.SendAsync(req);
                }
            }
            else
            {
                response = await client.SendAsync(req);
            }

            // Wirft einen Fehler wenn der Serviceaufruf einen Fehler am Server generierte
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength.GetValueOrDefault(0) > 0)
            {
                var ser = new DataContractSerializer(typeof(T));
                var obj = ser.ReadObject(await response.Content.ReadAsStreamAsync());
                return (T)obj;
            }

            return default(T);
        }

        #endregion
    }
}
