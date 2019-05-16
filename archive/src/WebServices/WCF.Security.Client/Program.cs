using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCF.Security.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunTransportSecurityAsync().Wait();
            RunCustomSecurityAsync().Wait();

            Console.WriteLine("Press the [any] Key");
            Console.ReadKey();
        }

        private static async Task RunTransportSecurityAsync()
        {
            using (var transportClient = new TransportSecurity.TransportSecurityServiceClient())
            {
                Console.WriteLine(await transportClient.DoWorkAsync());
            }
        }

        private static async Task RunCustomSecurityAsync()
        {
            using (var customSecurity = new CustomSecurity.CustomSecurityServiceClient())
            {
                customSecurity.ClientCredentials.UserName.UserName = "Flo";
                customSecurity.ClientCredentials.UserName.Password = "Boris";
                Console.WriteLine(await customSecurity.DoWorkAsync());
            }
        }
    }
}
