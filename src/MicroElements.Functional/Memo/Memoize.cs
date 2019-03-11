using System;
using System.Collections.Concurrent;

namespace MicroElements.Functional
{
    //https://stackoverflow.com/questions/2852161/c-sharp-memoization-of-functions-with-arbitrary-number-of-arguments
    public static partial class Prelude
    {
        /// <summary>
        /// Returns func that evaluates only once.
        /// All subsequent calls will return cached result.
        /// </summary>
        /// <typeparam name="R">Result type.</typeparam>
        /// <param name="func">Source func.</param>
        /// <returns>Memoized func.</returns>
        public static Func<R> Memoize<R>(Func<R> func)
        {
            var sync = new object();
            var result = default(R);
            bool isResultEvaluated = false;

            return () =>
            {
                if (isResultEvaluated)
                    return result;

                lock (sync)
                {
                    if (isResultEvaluated)
                        return result;

                    // Result evaluation.
                    result = func();
                    isResultEvaluated = true;
                    return result;
                }
            };
        }

        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     No mechanism for freeing cached values and therefore can cause a
        ///     memory leak when holding onto the Func<T,R> reference.
        ///     Uses a ConcurrentDictionary for the cache and is thread-safe
        /// </summary>
        public static Func<A, R> MemoizeUnsafe<A, R>(this Func<A, R> func)
        {
            var cache = new ConcurrentDictionary<A, R>();
            var syncMap = new ConcurrentDictionary<A, object>();
            return a =>
            {
                R r;
                if (!cache.TryGetValue(a, out r))
                {
                    var sync = syncMap.GetOrAdd(a, new object());
                    lock (sync)
                    {
                        r = cache.GetOrAdd(a, func);
                    }
                    syncMap.TryRemove(a, out sync);
                }
                return r;
            };
        }
    }

    public static class MemoExtensions
    {
        public static Func<A> Memoize<A>(this Func<A> func) => Memoize<A>(func);
    }

    //public interface ICache<TKey, TValue>
    //{
    //    TValue GetOrAdd(TKey key, Func<TKey, TValue> factory);
    //}

    //public class ConcurrentDictionaryCache<TKey, TValue> : ICache<TKey, TValue>
    //{
    //    private ConcurrentDictionary<TKey, TValue> _dictionary = new ConcurrentDictionary<TKey, TValue>();

    //    /// <inheritdoc />
    //    public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
    //    {
    //        return _dictionary.GetOrAdd(key, factory);
    //    }
    //}
}
