using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient.Cache
{
    public interface ICache<T>
    {
        Task<T> GetOrCreateAsync(string key, Func<CancellationToken, Task<Tuple<T, TimeSpan>>> factory, CancellationToken token);
    }
}
