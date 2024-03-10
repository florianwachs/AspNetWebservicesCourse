using System;

namespace ChuckNorrisService.Client
{
    public enum ApiJokeCategories
    {
        Dev,
        Food,
        Movie,
    }

    public static class ApiJokeCategoriesExtensions
    {
        public static string ToApiCategoryParameter(this ApiJokeCategories category)
        {
            // Man könnte auch mit .ToString() arbeiten, aber dann könnte beim
            // refaktoring des Enums möglicherweise eine Umbenennung stattfinden,
            // welche den API-Vertrag bricht.
            // Deshalb sicherheitshalber explizit die Umwandlung angeben.
            switch (category)
            {
                case ApiJokeCategories.Dev:
                    return "dev";
                case ApiJokeCategories.Food:
                    return "food";
                case ApiJokeCategories.Movie:
                    return "movie";
                default:
                    throw new InvalidOperationException("unknown joke category");
            }
        }
    }

}
