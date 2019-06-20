using System;
using System.Collections.Generic;
using System.Linq;
using Unit = System.ValueTuple;

namespace AspNetCoreTesting.Domain.Domain
{
    public static class Validation
    {
        public static Validation<Unit> Success()
        {
            return Validation<Unit>.Success(new Unit());
        }

        public static Validation<Unit> Fail(params Error[] errors)
        {
            return Validation<Unit>.Fail(errors);
        }
    }

    //Thanks @ https://github.com/la-yumba/functional-csharp-code 
    public struct Validation<T>
    {
        internal IEnumerable<Error> Errors { get; }
        internal T Value { get; }

        public bool IsValid { get; }

        public static Validation<T> Fail(IEnumerable<Error> errors)
           => new Validation<T>(errors);

        public static Validation<T> Fail(params Error[] errors)
           => new Validation<T>(errors.AsEnumerable());

        public static Validation<T> Success(T result)
        {
            return new Validation<T>(result);
        }

        private Validation(IEnumerable<Error> errors)
        {
            IsValid = false;
            Errors = errors;
            Value = default(T);
        }

        internal Validation(T right)
        {
            IsValid = true;
            Value = right;
            Errors = Enumerable.Empty<Error>();
        }

        public TR Match<TR>(Func<IEnumerable<Error>, TR> Invalid, Func<T, TR> Valid)
           => IsValid ? Valid(Value) : Invalid(Errors);

        public T ValueOrThrow() => IsValid ? Value : throw new InvalidOperationException("Validation has Errors");

        public IEnumerator<T> AsEnumerable()
        {
            if (IsValid)
            {
                yield return Value;
            }
        }

        public override string ToString()
           => IsValid
              ? $"Valid({Value})"
              : $"Invalid([{string.Join(", ", Errors)}])";

        public override bool Equals(object obj) => ToString() == obj.ToString(); // hack
    }

    public class Error
    {
        public virtual string Message { get; }
        public override string ToString() => Message;
        protected Error() { }
        internal Error(string Message) => this.Message = Message;

        public static implicit operator Error(string m) => new Error(m);
    }
}
