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
    public class QuizService : IQuizService
    {
        #region Fragen und Antworten
        //Der Fragenpool
        List<QuizFrage> fragen = new List<QuizFrage>();

        //Die möglichen Resultate einer Beantwortung
        private static readonly QuizResultat AntwortKorrekt = new QuizResultat("Deine Antwort stimmt! Prima! Weiter so!", true);
        private static readonly QuizResultat AntwortFalsch = new QuizResultat("Deine Antwort stimmt leider nicht!", false);
        private static readonly QuizResultat AntwortUnbekannt = new QuizResultat("Fehler! Sie haben auf eine nicht gestellte Frage geantwortet!", false);
        #endregion

        public QuizService()
        {
            fragen.Add(new QuizFrage(1, "Wie lautet ein wichtiger Standard bei Webservices?", new string[] { "WSS", "DSL", "WSDL" }, 2));
            fragen.Add(new QuizFrage(2, "Wie heisst der derzeitige Chef des W3Cs?", new string[] { "Tim Barners-Lee", "Bill Gates", "George W. Bush" }, 0));
            fragen.Add(new QuizFrage(3, "Was bedeutet SOAP?", new string[] { "Simple Object Access Protocol", "Service Oriented Architecture Protocol", "Secure Online Access Protocol" }, 0));
            fragen.Add(new QuizFrage(4, "In welchem Jahr wurde SVG als W3C Standard gekürt?", new string[] { "2000", "2001", "2002" }, 1));
        }

        public QuizFrage HoleFrage()
        {
            Random r = new Random();
            return fragen[r.Next(fragen.Count)];
        }

        public QuizResultat BeantworteFrage(int frageID, int antwortIndex)
        {
            QuizFrage aktFrage = fragen.Where(f => f.ID == frageID).FirstOrDefault();
            QuizResultat result = AntwortUnbekannt;

            if (aktFrage != null)
            {
                result = aktFrage.IstRichtig(antwortIndex) ? AntwortKorrekt : AntwortFalsch;
            }

            return result;
        }
    }
}
