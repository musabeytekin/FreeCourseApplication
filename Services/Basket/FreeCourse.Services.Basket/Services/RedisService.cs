using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Services;

public class RedisService
{
    private readonly string _host;
    private readonly int _port;
    
    private ConnectionMultiplexer _redisConnection;
    
    public RedisService(string host, int port)
    {
        _host = host;
        _port = port;
    }
    
    public void Connect() => _redisConnection = ConnectionMultiplexer.Connect($"{_host}:{_port}");
    public IDatabase GetDb(int db = 1) => _redisConnection.GetDatabase(db);
}