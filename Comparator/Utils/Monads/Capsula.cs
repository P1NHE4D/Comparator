using System;

namespace Comparator.Utils.Monads
{
    public abstract class Capsule<T>
    {
        public abstract Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func);

        public abstract Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func);

        public abstract T Return(T defaultValue);
    }

    public sealed class Success<T> : Capsule<T>
    {
        private readonly T _value;

        public Success(T value)
        {
            this._value = value;
        }

        public override Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func)
        {
            return func(this._value);
        }

        public override Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func)
        {
            return new Success<TReturn>(func(this._value));
        }

        public override T Return(T defaultValue)
        {
            return this._value;
        }
    }

    public sealed class Failure<T> : Capsule<T>
    {
        private readonly string _message;

        public Failure(string message)
        {
            this._message = message;
        }

        public override Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func)
        {
            return new Failure<TReturn>(_message);
        }

        public override Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func)
        {
            return new Failure<TReturn>(_message);
        }

        public override T Return(T defaultValue)
        {
            return defaultValue;
        }
    }

}