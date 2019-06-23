using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreRelationSample.Domain
{
    public class SharedList
    {
        public string UserId { get; private set; }
        public string ListId { get; private set; }
        public User User { get; private set; }
        public List List { get; private set; }
    }
}
