using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Portal.Helpers
{
    public class Caching
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly MemoryCache _cache = MemoryCache.Default;

        public T Get<T>(string key) where T : class
        {
            return _cache[key] as T;
        }

        public async Task UpdateAsync<T>(string key, Func<T, Task<T>> updateAction, TimeSpan timeout) where T : class
        {
            var keylock = _locks.GetOrAdd(key, x => new SemaphoreSlim(1));
            await keylock.WaitAsync();

            try
            {
                var item = Get<T>(key) ?? Activator.CreateInstance<T>();
                item = await updateAction(item);
                _cache.Set(key, item, DateTimeOffset.Now.Add(timeout));
            }
            finally
            {
                keylock.Release();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> refreshFunc, TimeSpan timeout, bool forceRefresh = false) where T : class
        {
            var item = Get<T>(key);
            if (item != null && !forceRefresh)
            {
                return item;
            }

            var keyLock = _locks.GetOrAdd(key, x => new SemaphoreSlim(1));
            await keyLock.WaitAsync();
            try
            {
                item = Get<T>(key);
                if (item == null || forceRefresh)
                {
                    item = await refreshFunc();
                    _cache.Set(key, item, DateTimeOffset.Now.Add(timeout));
                }

                return item;
            }
            finally
            {
                keyLock.Release();
            }
        }
    }
}