using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string CacheKey);
        Task SetAsync(string CacheKey, object Value, TimeSpan TimeToLive);


        public Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan timeToLive);
    }
}
