using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF.Soap.Server.Domain
{
    ///<summary>
    ///Datenstruktur einer Quizfrage, wie sie vom
    ///Client angefordert werden kann
    ///</summary>
    public class QuizFrage
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Frage { get; set; }
        [DataMember]
        public string[] Antworten { get; set; }
        [DataMember]
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