using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreRelationSample.Domain
{
    public class User
    {
        public string Id { get; set; }

        private List<SharedList> _sharedLists = new List<SharedList>();
        public IReadOnlyCollection<SharedList> SharedLists => _sharedLists.AsReadOnly();
    }
}
