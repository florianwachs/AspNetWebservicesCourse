using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WCF.Rest.ClientXml.Dtos
{
    [DataContract(Namespace = "http://www.fh-rosenheim.de/schema/dtos")]
    public class Person
    {
        [DataMember]
        public string Age { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
