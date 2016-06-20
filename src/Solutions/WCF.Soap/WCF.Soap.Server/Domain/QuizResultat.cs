using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCF.Soap.Server.Domain
{
    ///<summary>
    ///Datenstruktur einer Antwort auf die Beantwortung einer
    ///Quizfrage wie sie vom Server an den Client zurückgegeben wird
    ///</summary>
    [DataContract]
    public class QuizResultat
    {
        [DataMember]
        public string Antwort { get; set; }
        [DataMember]
        public bool Richtig { get; set; }

        public QuizResultat() { }

        public QuizResultat(string antwort, bool richtig)
        {
            Antwort = antwort;
            Richtig = richtig;
        }
    }
}