using DomainLayer.Contracts;
using ServiceAbstraction;
using System.Text.Json;

namespace Service
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
    }
}
