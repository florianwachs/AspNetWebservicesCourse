using Xunit;

namespace Advanced;


public class Records
{
    [Fact]
    public void Demo1()
    {
        var person1 = new PersonClass { FirstName = "Chuck", LastName = "Norris" };
        var person2 = new PersonClass { FirstName = "Chuck", LastName = "Norris" };

        Assert.NotEqual(person1, person2);
    }

    [Fact]
    public void Demo2()
    {
        var person1 = new PersonRecord { FirstName = "Chuck", LastName = "Norris" };
        var person2 = new PersonRecord { FirstName = "Chuck", LastName = "Norris" };

        Assert.Equal(person1, person2);
    }

    [Fact]
    public void Demo3()
    {
        var person1 = new PersonWithPrimaryConstructor("Chuck", "Norris");
        var person2 = new PersonWithPrimaryConstructor("Chuck", "Norris");

        Assert.Equal(person1, person2);
    }

    [Fact]
    public void Demo4()
    {
        var person1 = new PersonWithPrimaryConstructor("Chuck", "Norris");
        var person2 = person1 with { FirstName = "Amanda" };

        Assert.Equal("Amanda", person2.FirstName);
    }
}


public class PersonClass
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

public record PersonRecord
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

public record PersonWithPrimaryConstructor(string FirstName, string LastName);