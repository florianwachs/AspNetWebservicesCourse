using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF.Security
{
    [ServiceContract]
    public interface ITransportSecurityService
    {
        [OperationContract]
        string DoWork();
    }
}
