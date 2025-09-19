using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Common
{
    public interface ICacheService :IDisposable
    {
        T Get<T>(string key);
        bool TryGetValue<T>(string key, out T value);
        void Set<T>(string key, T value, TimeSpan expiry, int size = 1);
        void Remove(string key);
        void Clear();
        T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory);
        Task<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory);
    }
}
