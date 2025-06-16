using StackExchange.Redis;

namespace OrderService.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConfiguration config)
        {
            var redis = ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]);
            _database = redis.GetDatabase();
        }

        public async Task SetValueAsync(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }

        public async Task<string?> GetValueAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }
    }
}
