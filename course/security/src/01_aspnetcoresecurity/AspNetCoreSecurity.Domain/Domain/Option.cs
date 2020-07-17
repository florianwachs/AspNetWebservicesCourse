using System;
using System.Collections.Generic;

namespace AspNetCoreSecurity.Domain.Domain
{
    //Thanks @ https://github.com/la-yumba/functional-csharp-code 
    public struct Option<T> : IEquatable<Option.None>, IEquatable<Option<T>>
    {
        private readonly T value;
        public T Value => IsSome ? value : throw new InvalidOperationException("Option is None");
        public bool IsSome { get; }

        public bool IsNone => !IsSome;

        private Option(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            IsSome = true;
            this.value = value;
        }

        public static implicit operator Option<T>(Option.None _) => new Option<T>();
        public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);

        public static implicit operator Option<T>(T value)
           => value == null ? (Option<T>)Option.None.Default : new Option.Some<T>(value);

        public R Match<R>(Func<R> None, Func<T, R> Some)
            => IsSome ? Some(value) : None();

        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return value;
            }
        }

        public bool Equals(Option<T> other)
           => IsSome == other.IsSome
           && (IsNone || value.Equals(other.value));

        public bool Equals(Option.None _) => IsNone;

        public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);
        public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

        public override string ToString() => IsSome ? $"Some({value})" : "None";
    }

    namespace Option
    {
        public struct None
        {
            internal static readonly None Default = new None();
        }

        public struct Some<T>
        {
            internal T Value { get; }

            internal Some(T value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value)
                       , "Cannot wrap a null value in a 'Some'; use 'None' instead");
                }

                Value = value;
            }
        }
    }
}
