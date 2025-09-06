using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.AWS.Contrato
{
    public interface ICacheAWS
    {
        Task<T?> GetCacheAsync<T>(string cacheKey) where T : class;
        Task SetCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null) where T : class;
        Task DeleteCacheAsync(string cacheKey);
        Task<bool> ExistsCacheAsync(string cacheKey);

    }
}