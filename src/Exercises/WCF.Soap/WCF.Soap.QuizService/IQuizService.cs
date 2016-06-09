using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCF.Soap.QuizService.Domain;

namespace WCF.Soap.QuizService
{
    public interface IQuizService
    {
        QuizFrage HoleFrage();
        QuizResultat BeantworteFrage(int frageID, int antwortIndex);
    }
}
