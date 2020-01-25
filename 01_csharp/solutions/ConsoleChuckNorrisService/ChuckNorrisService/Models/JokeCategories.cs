using System;

namespace ChuckNorrisService.Models
{
    public enum JokeCategories
    {
        Dev,
        Food,
        Movie,
    }

    public static class JokeCategoriesExtensions
    {
        public static string ToApiCategoryParameter(this JokeCategories category)
        {
            // Man könnte auch mit .ToString() arbeiten, aber dann könnte beim
            // refaktoring des Enums möglicherweise eine Umbenennung stattfinden,
            // welche den API-Vertrag bricht.
            // Deshalb sicherheitshalber explizit die Umwandlung angeben.
            return category switch
            {
                JokeCategories.Dev => "dev",
                JokeCategories.Food => "food",
                JokeCategories.Movie => "movie",
                _ => throw new InvalidOperationException("unknown joke category")
            };
        }
    }
}
