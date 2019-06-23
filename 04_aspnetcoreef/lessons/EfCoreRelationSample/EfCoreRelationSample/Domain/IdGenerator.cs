using System;

namespace EfCoreRelationSample.Domain
{
    public class IdGenerator
    {
        public string NewEntityId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
