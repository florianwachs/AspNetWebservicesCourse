using Xunit;

namespace Basics;

// Von static Klassen gibt es genau eine Instanz pro AppDomain
public static class StaticClass
{
    [Fact]
    public static void Demo1()
    {
        Console.WriteLine(GetUserName());
    }

    // in einer static class müssen alle Member static sein
    private static string GetUserName()
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
