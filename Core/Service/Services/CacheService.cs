using DomainLayer.Contracts;
using ServiceAbstraction.Abstractions;
using System.Text.Json;

namespace Service.Services
{
    public class CacheService(ICacheRepository _cacheRepository) : ICacheService
    {
        public async Task<string?> GetAsync(string CacheKey)
            => await _cacheRepository.GetAsync(CacheKey);
        public async Task SetAsync(string CacheKey, object Value, TimeSpan TimeToLive)
        {
            var CacheValue = JsonSerializer.Serialize(Value);
            await _cacheRepository.SetAsync(CacheKey, CacheValue, TimeToLive);
        }


        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cacheRepository.GetAsync(key);

            if (value is null)
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan timeToLive)
        {
            var jsonValue = JsonSerializer.Serialize(value);
            await _cacheRepository.SetAsync(key, jsonValue, timeToLive);
        }
    }
}
