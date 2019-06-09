using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public class Kg : IEquatable<Kg>, IComparable<Kg>
    {
        public static readonly Kg Zero = new Kg(0);
        public decimal Amount { get; private set; }

        private Kg()
        {
            // For EF Core to hydrate entity from database
        }

        public Kg(decimal amount)
        {
            if (Amount < 0)
                throw new InvalidOperationException("there are no negative kgs");

            Amount = amount;
        }

        public static Kg Create(decimal amount)
        {
            return new Kg(amount);
        }

        public override string ToString()
        {
            return $"{Amount} Kg";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Kg);
        }

        public bool Equals(Kg other)
        {
            return other != null &&
                   Amount == other.Amount;
        }

        public override int GetHashCode()
        {
            return -602769199 + Amount.GetHashCode();
        }

        public int CompareTo(Kg other)
        {
            return Amount.CompareTo(other.Amount);
        }

        public static bool operator ==(Kg left, Kg right)
        {
            return EqualityComparer<Kg>.Default.Equals(left, right);
        }

        public static bool operator !=(Kg left, Kg right)
        {
            return !(left == right);
        }

        public static Kg operator +(Kg left, Kg right)
        {
            return new Kg(left.Amount + right.Amount);
        }
    }
}
