namespace AspNetCoreHateoasWithLinks.Models
{
    public class LinkDto
    {
        // Verwendungszweck / Aktion
        public string Rel { get; }

        // URL zur „Aktion“
        public string Href { get; }

        // zu verwendendes HTTP-Verb
        public string Method { get; }

        public LinkDto(string rel, string href, string method)
        {
            // TODO Validation
            Rel = rel;
            Href = href;
            Method = method;
        }
    }

}
