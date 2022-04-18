namespace Advanced;

public class ExpressionBodiedFunctionAndProperty
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // Normale berechnete Property
    public string CompleteName
    {
        get
        {
            return FirstName + LastName;
        }
    }

    // Property mit Expression-Body
    public string CompleteName2 => FirstName + LastName;

    // Normale Methode
    public string CalculateBestHero(int inYear)
    {
        return "Chuck Norris! EVERY YEAR";
    }

    // Methode mit Expression-Body
    public string CalculateBestHero2(int inYear) => "Chuck Norris! EVERY YEAR";
}
