using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomapperLesson.AccountManagment.ViewModels
{
    public class UserProfileEditModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string CreditCardNumber { get; set; }
    }
}
