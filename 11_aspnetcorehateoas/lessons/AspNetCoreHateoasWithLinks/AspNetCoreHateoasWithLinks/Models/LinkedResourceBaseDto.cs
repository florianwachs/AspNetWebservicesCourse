using System.Collections.Generic;

namespace AspNetCoreHateoasWithLinks.Models
{
    public abstract class LinkedResourceBaseDto
    {
        private readonly List<LinkDto> _links;
        public IReadOnlyCollection<LinkDto> Links => _links.AsReadOnly();

        public LinkedResourceBaseDto()
        {
            _links = new List<LinkDto>();
        }

        public void Add(LinkDto link)
        {
            _links.Add(link);
        }
    }
}
