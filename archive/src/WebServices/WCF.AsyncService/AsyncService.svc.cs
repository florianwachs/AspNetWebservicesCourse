using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCF.AsyncService
{
    public class AsyncService : IAsyncService
    {
        public IAsyncResult BeginAnswerQuestion(string question, AsyncCallback callback, object state)
        {
            Debug.WriteLine("Begin on Thread " + Thread.CurrentThread.ManagedThreadId);
            // Erzeugt einen Timer in einer .NET Threadpool Thread 
            var task = Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith((t, s) =>
            {
                Debug.WriteLine("Working on Thread " + Thread.CurrentThread.ManagedThreadId);
                return "42";
            }, state);

            // Wenn der Task fertig ist, wird der callback aufgerufen
            // damit wird signalisiert das EndAnswerQuestion aufgerufen
            // werden kann um das Ergebnis an den Client zu liefern
            return task.ContinueWith(res => callback(res));
        }

        // Wird aufgerufen sobald die Async-Operation fertig ist
        public string EndAnswerQuestion(IAsyncResult result)
        {
            Debug.WriteLine("End on Thread " + Thread.CurrentThread.ManagedThreadId);
            return ((Task<string>)result).Result;
        }


        public async Task<string> AnswerTask(string question)
        {
            Debug.WriteLine("Begin on Thread " + Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Debug.WriteLine("End on Thread " + Thread.CurrentThread.ManagedThreadId);
            return "42";
        }

        public string AnswerSynchron(string question)
        {
            var task = Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ =>
            {
                return "42";
            });

            // Der Aufruf blockiert den WCF-Worker bis das Ergebnis vorliegt
            return task.Result;
        }
    }
}
