using Xunit;

namespace Basics;

public class Collections
{
    [Fact]
    public static void DemoList()
    {
        var list = new List<int>();
        list.Add(1);
        list.AddRange(new[] { 2, 3, 4, 5 });

        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Console.WriteLine(item);
        }

        foreach (var item in list)
        {
            Console.WriteLine(item);
        }
    }

    [Fact]
    public void DemoDictionary()
    {
        var map = new Dictionary<string, decimal>
        {
            { "EUR", 1 },
            { "USD", 1.1m },
            { "NOK", 8m }
        };

        map.Add("CHF", 2);

        var quote = map["CHF"];
        map["CHF"] = 2.5m;

        if (map.ContainsKey("AUD")) { }

        decimal q;
        if (!map.TryGetValue("AUD", out q))
        {
            q = 10;
        }

        // C# 7: out-Variables
        // amount ist nur innerhalb des if-Blocks zugewiesen
        if (map.TryGetValue("USD", out decimal amount))
        {
            q = amount;
        }
    }
}
