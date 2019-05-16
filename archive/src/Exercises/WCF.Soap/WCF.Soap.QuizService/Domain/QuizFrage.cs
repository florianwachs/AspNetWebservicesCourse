using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF.Soap.QuizService.Domain
{
    ///<summary>
    ///Datenstruktur einer Quizfrage, wie sie vom
    ///Client angefordert werden kann
    ///</summary>
    public class QuizFrage
    {
        public int ID { get; set; }
        public string Frage { get; set; }
        public string[] Antworten { get; set; }
        private int IdxRichtigeAntwort { get; set; }

        public QuizFrage() { }

        public QuizFrage(int id, string frage, string[] antworten, int idxRichtigeAntwort)
        {
            ID = id;
            Frage = frage;
            Antworten = antworten;
            IdxRichtigeAntwort = idxRichtigeAntwort;
        }

        public bool IstRichtig(int idx)
        {
            if (idx == IdxRichtigeAntwort) return true;
            return false;
        }
    }
}