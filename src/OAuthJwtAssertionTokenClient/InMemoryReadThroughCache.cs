using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace OAuthJwtAssertionTokenClient
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class InMemoryReadThroughCache : IReadThroughCache, IDisposable
    {
        private readonly MemoryCache _cache = new MemoryCache("InMemoryReadThroughCache");
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public Task<T> GetAsync<T>(string cacheKey, Func<Task<Tuple<T, TimeSpan>>> underlyingSource)
        {
            var cacheEntry = (T)_cache.Get(cacheKey);
            return cacheEntry != null
                ? Task.FromResult(cacheEntry)
                : AddAndGet(cacheKey, underlyingSource);
        }

        private async Task<T> AddAndGet<T>(string cacheKey, Func<Task<Tuple<T, TimeSpan>>> underlyingSource)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var token = (T) _cache.Get(cacheKey);
                if (token == null)
                {
                    var expiringToken = await underlyingSource().ConfigureAwait(false);
                    token = expiringToken.Item1;
                    var validUntil = DateTimeOffset.UtcNow.Add(expiringToken.Item2);
                    _cache.Add(cacheKey, token, validUntil);
                }
                return token;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #region IDisposable

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~InMemoryReadThroughCache()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _cache.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
