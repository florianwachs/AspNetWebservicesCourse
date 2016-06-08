using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF.Soap.Dtos
{
    [DataContract]
    public class Student
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }
}