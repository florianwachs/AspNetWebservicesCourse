using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCF.Soap.Server.Domain;

namespace WCF.Soap.Server
{
    [ServiceContract]
    public interface IQuizService
    {
        [OperationContract]
        QuizFrage HoleFrage();
        [OperationContract]
        QuizResultat BeantworteFrage(int frageID, int antwortIndex);
    }
}
