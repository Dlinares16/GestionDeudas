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
            _tableName = configuration["DynamoDb:CacheTableName"] ?? "DebtCache";
        }

        /// <summary>
        /// Obtiene datos del caché por clave
        /// </summary>
        /// <typeparam name="T">Tipo de datos a obtener</typeparam>
        /// <param name="cacheKey">Clave única del caché</param>
        /// <returns>Datos deserializados o null si no existen/expiraron</returns>
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
                            _ = Task.Run(() => DeleteCacheAsync(cacheKey));
                            return null;
                        }
                    }
                }

                // Si llegamos aquí, tenemos datos válidos
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

        /// <summary>
        /// Guarda datos en el caché
        /// </summary>
        /// <typeparam name="T">Tipo de datos a guardar</typeparam>
        /// <param name="cacheKey">Clave única del caché</param>
        /// <param name="data">Datos a guardar</param>
        /// <param name="expiration">Tiempo de expiración opcional</param>
        public async Task SetCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null) where T : class
        {
            try
            {
                if (data == null)
                {
                    return;
                }
                var item = new Dictionary<string, AttributeValue>
                {
                    { "CacheKey", new AttributeValue { S = cacheKey } },
                    { "Data", new AttributeValue { S = JsonSerializer.Serialize(data) } },
                    { "CreatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("O") } },
                    { "UpdatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("O") } }
                };

                // Agregar tiempo de expiración si se especifica
                if (expiration.HasValue)
                {
                    var expiresAt = DateTime.UtcNow.Add(expiration.Value);
                    item["ExpiresAt"] = new AttributeValue { S = expiresAt.ToString("O") };

                    // También agregar TTL para DynamoDB (opcional, requiere configurar TTL en la tabla)
                    item["TTL"] = new AttributeValue { N = ((DateTimeOffset)expiresAt).ToUnixTimeSeconds().ToString() };
                }

                var request = new PutItemRequest
                {
                    TableName = _tableName,
                    Item = item
                };

                await _dynamoDb.PutItemAsync(request);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        /// <summary>
        /// Elimina datos del caché
        /// </summary>
        /// <param name="cacheKey">Clave del caché a eliminar</param>
        public async Task DeleteCacheAsync(string cacheKey)
        {
            try
            {
                var request = new DeleteItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "CacheKey", new AttributeValue { S = cacheKey } }
                    }
                };

                await _dynamoDb.DeleteItemAsync(request);
            }
            catch (Exception ex)
            {
                // No lanzar excepción
            }
        }

        /// <summary>
        /// Verifica si existe una clave en el caché
        /// </summary>
        /// <param name="cacheKey">Clave a verificar</param>
        /// <returns>True si existe y no ha expirado, False en caso contrario</returns>
        public async Task<bool> ExistsCacheAsync(string cacheKey)
        {
            try
            {
                var data = await GetCacheAsync<object>(cacheKey);
                return data != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Limpia entradas expiradas del caché (método de mantenimiento)
        /// </summary>
        /// <returns>Número de entradas eliminadas</returns>
        public async Task<int> CleanupExpiredEntriesAsync()
        {
            try
            {
                var scanRequest = new ScanRequest
                {
                    TableName = _tableName,
                    FilterExpression = "ExpiresAt < :currentTime",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":currentTime", new AttributeValue { S = DateTime.UtcNow.ToString("O") } }
                    }
                };

                var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
                var expiredItems = scanResponse.Items;

                int deletedCount = 0;
                foreach (var item in expiredItems)
                {
                    try
                    {
                        var deleteRequest = new DeleteItemRequest
                        {
                            TableName = _tableName,
                            Key = new Dictionary<string, AttributeValue>
                            {
                                { "CacheKey", item["CacheKey"] }
                            }
                        };

                        await _dynamoDb.DeleteItemAsync(deleteRequest);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }

                return deletedCount;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }

    public static class CacheKeys
    {
        public static string UserById(int userId) => $"user_by_id_{userId}";
        public static string UserByEmail(string email) => $"user_by_email_{email.ToLower()}";
        public static string UserDebts(int userId) => $"user_debts_{userId}";
        public static string UserDebtsPaid(int userId) => $"user_debts_paid_{userId}";
        public static string UserDebtsPending(int userId) => $"user_debts_pending_{userId}";
        public static string UserDebtStats(int userId) => $"user_debt_stats_{userId}";
        public static string DebtById(int debtId) => $"debt_by_id_{debtId}";
    }
}
