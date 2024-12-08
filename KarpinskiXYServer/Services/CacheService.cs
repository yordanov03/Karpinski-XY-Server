using Karpinski_XY_Server.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Karpinski_XY_Server.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;

        // Default expiration settings
        private static readonly TimeSpan DefaultAbsoluteExpiration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan DefaultSlidingExpiration = TimeSpan.FromMinutes(10);

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T value))
            {
                _logger.LogInformation($"Cache hit for key: {key}");
                return value;
            }

            _logger.LogInformation($"Cache miss for key: {key}");
            return default;
        }

        public void Set<T>(string key, T value)
        {
            Set(key, value, DefaultAbsoluteExpiration, DefaultSlidingExpiration);
        }

        public void Set<T>(string key, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };

            _memoryCache.Set(key, value, cacheOptions);
            _logger.LogInformation($"Cache set for key: {key} with absolute expiration: {absoluteExpiration} and sliding expiration: {slidingExpiration}");
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation($"Cache removed for key: {key}");
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                _memoryCache.Remove(key);
                _logger.LogInformation($"Cache removed for key: {key}");
            }
        }
    }
}
