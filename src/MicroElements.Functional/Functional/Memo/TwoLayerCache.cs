// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents cache that holds only limited number of items.
    /// Cache organized in two layers: hot and cold. Items first added to cold cache.
    /// GetValue first checks hot cache. If value not found in hot cache than cold cache uses for search.
    /// If value exists in cold cache than item moves to hot cache.
    /// If hot cache exceeds item limit then hot cache became cold cache and new hot cache creates.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class TwoLayerCache<TKey, TValue>
    {
        private readonly int _maxItemCount;
        private readonly object _sync = new object();
        private ConcurrentDictionary<TKey, TValue> _hotCache = new ConcurrentDictionary<TKey, TValue>();
        private ConcurrentDictionary<TKey, TValue> _coldCache = new ConcurrentDictionary<TKey, TValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoLayerCache{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="maxItemCount">Max item count.</param>
        public TwoLayerCache(int maxItemCount)
        {
            _maxItemCount = maxItemCount;
        }

        /// <summary>
        /// Adds item to cache.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void Add(TKey key, TValue value)
        {
            // Add items only to cold cache.
            _coldCache.TryAdd(key, value);
        }

        /// <summary>
        /// Gets item by key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Found value or default.</param>
        /// <returns>true if value found by key.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            // Try get value from hot cache.
            if (_hotCache.TryGetValue(key, out value))
            {
                // Cache hit, no more actions.
                return true;
            }

            // Check whether value exists in cold cache.
            if (_coldCache.TryGetValue(key, out value))
            {
                // Value exists in cold cache so move to hot cache.
                _hotCache.TryAdd(key, value);
                // If not remove from cold then cold cache size can be twice as hot.
                //_coldCache.TryRemove(key, out _);

                // If hot cache exceeds limit then move all to cold cache and create new hot cache.
                if (_hotCache.Count > _maxItemCount)
                {
                    lock (_sync)
                    {
                        if (_hotCache.Count > _maxItemCount)
                        {
                            _coldCache = _hotCache;
                            _hotCache = new ConcurrentDictionary<TKey, TValue>();
                        }
                    }
                }

                return true;
            }

            value = default;
            return false;
        }
    }
}
