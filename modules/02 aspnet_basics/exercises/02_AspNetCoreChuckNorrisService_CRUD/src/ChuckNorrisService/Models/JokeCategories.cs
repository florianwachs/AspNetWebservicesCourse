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
            switch (category)
            {
                case JokeCategories.Dev:
                    return "dev";
                case JokeCategories.Food:
                    return "food";
                case JokeCategories.Movie:
                    return "movie";
                default:
                    throw new InvalidOperationException("unknown joke category");
            }
        }
    }
}
