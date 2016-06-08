using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    // Von static Klassen gibt es genau eine Instanz pro AppDomain
    public static class StaticClass
    {
        public static void Demo1()
        {
            Console.WriteLine(GetUserName());
        }

        // in einer static class müssen alle Member static sein
        public static string GetUserName()
        {
            return Environment.UserDomainName;
        }
    }

    public class StaticMemberInClass
    {
        // in einer non-static class können static Member enthalten sein
        public static string XmlElementName
        {
            get
            {
                return "StaticMember";
            }
        }
    }
}
