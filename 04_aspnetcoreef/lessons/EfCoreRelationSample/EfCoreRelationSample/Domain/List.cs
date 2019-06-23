using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreRelationSample.Domain
{
    public class List
    {
        public string Id { get; set; }
        private List<SharedList> _sharedWith = new List<SharedList>();
        public IReadOnlyCollection<SharedList> SharedWith => _sharedWith.AsReadOnly();
    }
}
