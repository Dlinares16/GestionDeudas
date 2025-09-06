using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using GestionDeudas.BLL.AWS.Contrato;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.AWS
{
    public class CacheAWS : ICacheAWS
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;

        public CacheAWS(
            IAmazonDynamoDB dynamoDb,
            IConfiguration configuration)
        {
            _dynamoDb = dynamoDb;
            _tableName = configuration.GetValue<string>("DynamoDb:CacheTableName") ?? "DebtCache";
        }

        public Task DeleteCacheAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsCacheAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> GetCacheAsync<T>(string cacheKey) where T : class
        {
            try
            {
                var request = new GetItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "CacheKey", new AttributeValue { S = cacheKey } }
                    }
                };

                var response = await _dynamoDb.GetItemAsync(request);

                // Si no se encontró el item
                if (!response.Item.Any())
                {
                    return null;
                }

                // Verificar si los datos han expirado
                if (response.Item.ContainsKey("ExpiresAt"))
                {
                    if (DateTime.TryParse(response.Item["ExpiresAt"].S, out var expiresAt))
                    {
                        if (DateTime.UtcNow > expiresAt)
                        {
                            // Eliminar datos expirados
                            _ = Task.Run(() => DeleteAsync(cacheKey));
                            return null;
                        }
                    }
                }

                // Tenemos datos válidos
                if (!response.Item.ContainsKey("Data"))
                {
                    return null;
                }

                var jsonData = response.Item["Data"].S;
                var deserializedData = JsonSerializer.Deserialize<T>(jsonData);

                return deserializedData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task SetCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
