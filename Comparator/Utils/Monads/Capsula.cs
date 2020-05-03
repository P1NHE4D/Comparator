using System;
using Comparator.Utils.Logger;

namespace Comparator.Utils.Monads {
    public abstract class Capsule<T> {
        public abstract Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func);
        public abstract Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func);
        public abstract T Catch(Func<string, T> func);
        public abstract T Return(T defaultValue);

        public static Capsule<T> CreateSuccess(T value) => new Success<T>(value);
        public static Capsule<T> CreateFailure(string message) => new Failure<T>(message);

        public Capsule<TResult> SelectMany<TValue2, TResult>(
            Func<T, Capsule<TValue2>> function,
            Func<T, TValue2, TResult> projection) {
            return Bind(
                outer => function(outer).Bind(
                    inner => new Success<TResult>(projection(outer, inner))));
        }
    }

    public sealed class Success<T> : Capsule<T> {
        private readonly T _value;

        public Success(T value) => _value = value;

        public override Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func) => func(_value);

        public override Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func) => new Success<TReturn>(func(_value));

        public override T Catch(Func<string, T> func) => _value;

        public override T Return(T defaultValue) => _value;
    }

    public sealed class Failure<T> : Capsule<T> {
        private readonly string _message;

        public Failure(string message) => _message = message;

        public Failure(string message, ILoggerManager logger) : this(message) => logger.LogError(message);

        public override Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func) =>
            new Failure<TReturn>(_message);

        public override Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func) => new Failure<TReturn>(_message);

        public override T Catch(Func<string, T> func) => func(_message);
        public override T Return(T defaultValue) => defaultValue;
    }
}