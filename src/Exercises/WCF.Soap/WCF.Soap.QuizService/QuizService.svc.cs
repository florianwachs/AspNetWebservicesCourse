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
            ///TODO Fragen initialisieren und Fragenpool erstellen!!

        }

        public QuizFrage HoleFrage()
        {
            Random r = new Random();
            return fragen[r.Next(fragen.Count)];
        }

        public QuizResultat BeantworteFrage(int frageID, int antwortIndex)
        {
            //Die Frage mit der angegebenen ID suchen 
            QuizFrage aktFrage = null;
            //TODO: Die Frage mit der angegebenen ID im Fragepool suchen 

            // TODO:
            //Prüfen, ob die angegebene Antwort richtig ist, bzw. ob überhaupt
            //auf eine vorhandene Frage geantwortet wurde. Entsprechendes Resultat
            //ausgeben
            throw new NotImplementedException();
        }
    }
}
