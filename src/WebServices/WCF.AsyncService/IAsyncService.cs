using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCF.AsyncService
{
    [ServiceContract]
    public interface IAsyncService
    {
        // Der Contract kann Sync und Async Methoden implementieren
        [OperationContract]
        string AnswerSynchron(string question);

        // Für Asynchrone Service Implementierungen muss bei WCF auf das APM-Pattern
        // zurückgegriffen werden. Dieses erfordert ein Methoden-Paar aus Begin und End
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAnswerQuestion(string question, AsyncCallback callback, object state);

        string EndAnswerQuestion(IAsyncResult result);

        [OperationContract]
        Task<string> AnswerTask(string question);
    }
}
