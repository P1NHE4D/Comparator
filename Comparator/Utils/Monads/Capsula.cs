using System;
using Comparator.Utils.Logger;
using Microsoft.AspNetCore.Http;

namespace Comparator.Utils.Monads {
    public abstract class Capsule<T> {
        public abstract Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func);
        public abstract Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func);
        public abstract Capsule<T> MapFailure(Func<Exception, Exception> func);
        public abstract T Catch(Func<Exception, T> func);
        public abstract T Return(T defaultValue);

        public abstract Capsule<T> Access(Action<T> action);

        public static Capsule<T> CreateSuccess(T value) => new Success<T>(value);
        public static Capsule<T> CreateFailure(string message) => new Failure<T>(message);

        public static Capsule<T> CreateFailure(string message, ILoggerManager logger) =>
            new Failure<T>(message, logger);

        public Capsule<TResult> SelectMany<TValue2, TResult>(
            Func<T, Capsule<TValue2>> function,
            Func<T, TValue2, TResult> projection) {
            return Bind(
                outer => function(outer).Bind(
                    inner => new Success<TResult>(projection(outer, inner))));
        }

        public Capsule<TReturn> Select<TReturn>(Func<T, TReturn> func) => this.Map(func);
    }

    public sealed class Success<T> : Capsule<T> {
        private readonly T _value;

        public Success(T value) => _value = value;

        public override Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func) {
            try {
                return func(_value);
            } catch (Exception e) {
                return Capsule<TReturn>.CreateFailure($"Status code: {StatusCodes.Status500InternalServerError}. ({e.Message})");
            }
        }

        public override Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func) {
            try {
                return new Success<TReturn>(func(_value));
            } catch (Exception e) {
                return Capsule<TReturn>.CreateFailure($"Status code: {StatusCodes.Status500InternalServerError}. ({e.Message})");
            }
        }

        public override Capsule<T> MapFailure(Func<Exception, Exception> func) => this;


        public override Capsule<T> Access(Action<T> action) {
            action(_value);
            return this;
        }

        public override T Catch(Func<Exception, T> func) => _value;

        public override T Return(T defaultValue) => _value;
    }

    public sealed class Failure<T> : Capsule<T> {
        private readonly Exception _exception;

        public Failure(string message) => _exception = new Exception(message);
        public Failure(Exception exception) => _exception = exception;

        public Failure(string message, ILoggerManager logger) : this(message) => logger.LogError(message);

        public override Capsule<TReturn> Bind<TReturn>(Func<T, Capsule<TReturn>> func) =>
            new Failure<TReturn>(_exception);

        public override Capsule<TReturn> Map<TReturn>(Func<T, TReturn> func) => new Failure<TReturn>(_exception);
        public override Capsule<T> MapFailure(Func<Exception, Exception> func) => new Failure<T>(func(_exception));

        public override Capsule<T> Access(Action<T> action) => this;

        public override T Catch(Func<Exception, T> func) => func(_exception);
        public override T Return(T defaultValue) => defaultValue;
    }
}