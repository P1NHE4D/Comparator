using System;
using System.Threading.Tasks;

namespace Comparator.Utils.Monads {
    public static class TaskExtensions {
        public static async Task<TResult> Bind<TValue, TResult>(this Task<TValue> task,
                                                                Func<TValue, Task<TResult>> func) {
            return await func(await task);
        }


        public static async Task<TResult> Select<TValue, TResult>(
            this Task<TValue> source, Func<TValue, TResult> selector) {
            var t = await source;
            return selector(t);
        }

        // Extensions for LINQ:
        public static Task<TResult> SelectMany<TValue1, TValue2, TResult>(
            this Task<TValue1> monad,
            Func<TValue1, Task<TValue2>> function,
            Func<TValue1, TValue2, TResult> projection) {
            return monad.Bind(
                outer => function(outer).Bind(
                    inner => Task.FromResult(projection(outer, inner))));
        }

        public static Task<Capsule<TResult>> SelectMany<TValue1, TValue2, TResult>(
            this Task<TValue1> task,
            Func<TValue1, Capsule<TValue2>> function,
            Func<TValue1, TValue2, TResult> projection) {
            return task.Bind(value1 =>
                                 Task.FromResult(function(value1).Map(value2 =>
                                                                          projection(value1, value2))));
        }
    }
}