using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public class Properties
    {
        // Properties mit Backing-Field
        private int age;
        public int Age
        {
            get
            {
                return age;
            }

            private set
            {
                age = value;
            }
        }

        // Readonly Properties mit Custom Logic
        public string AgeMessage
        {
            get
            {
                var message = "";

                if (age < 30)
                    message = "Genieße das Leben";
                else
                {
                    message = "Das Ende naht...";
                }

                return message;
            }
        }

        // auto-property
        public string FirstName { get; private set; }

        // C# 6 initializer
        public DateTime TimeStamp { get; } = DateTime.UtcNow;

        // C# 7 expression
        public DateTime Today => DateTime.Today;

        // C# 7 expression and normal mixed
        public int Age2
        {
            get => age;
            set
            {
                if (age >= 0)
                    age = value;
            }
        }
    }
}
