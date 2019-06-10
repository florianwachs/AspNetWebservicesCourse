using System;
using System.Collections.Generic;

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
            {
                throw new InvalidOperationException("there are no negative kgs");
            }

            Amount = amount;
        }

        public static Kg Create(decimal amount) => new Kg(amount);

        public override string ToString() => $"{Amount} Kg";

        public override bool Equals(object obj) => Equals(obj as Kg);

        public bool Equals(Kg other) => other != null &&
                   Amount == other.Amount;

        public override int GetHashCode() => -602769199 + Amount.GetHashCode();

        public int CompareTo(Kg other) => Amount.CompareTo(other.Amount);

        public static bool operator ==(Kg left, Kg right) => EqualityComparer<Kg>.Default.Equals(left, right);

        public static bool operator !=(Kg left, Kg right) => !(left == right);

        public static Kg operator +(Kg left, Kg right) => new Kg(left.Amount + right.Amount);

        public static bool operator <(Kg left, Kg right) => left.Amount < right.Amount;

        public static bool operator >(Kg left, Kg right) => left.Amount < right.Amount;
    }
}
