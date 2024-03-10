namespace Advanced;

public class Nameof
{
    public string FirstName { get; set; }

    public void UseNameof()
    {
        Console.WriteLine(nameof(CSharpAdvancedLanguageFeatures));  // CSharpAdvancedLanguageFeatures
        Console.WriteLine(nameof(FirstName));                       // FirstName
                                                                    // es wird nur der letzte Identifier ausgewertet
        Console.WriteLine(nameof(Console.WriteLine));               // WriteLine
    }

    public void WhoIsYourGreatestHero(string heroName)
    {
        if (heroName != "Chuck Norris")
            throw new ArgumentException("Der Parameter ist ungültig", nameof(heroName)); // wird nun bei Refactorings angepasst
    }
}
