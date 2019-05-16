using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF.Soap.QuizService.Domain
{
    ///<summary>
    ///Datenstruktur einer Antwort auf die Beantwortung einer
    ///Quizfrage wie sie vom Server an den Client zurückgegeben wird
    ///</summary>
    public class QuizResultat
    {
        public string Antwort { get; set; }
        public bool Richtig { get; set; }

        public QuizResultat() { }

        public QuizResultat(string antwort, bool richtig)
        {
            Antwort = antwort;
            Richtig = richtig;
        }
    }
}