using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Services
{
    // RedisService, Redis veritabanına bağlantı sağlamak ve veritabanı işlemlerini yönetmek için kullanılan bir servis sınıfıdır.
    public class RedisService
    {
        private readonly string _host; // Redis sunucusunun ana bilgisayar adı.
        private readonly int _port; // Redis sunucusunun port numarası.

        private ConnectionMultiplexer _ConnectionMultiplexer; // Redis'e bağlantı sağlamak için kullanılan ConnectionMultiplexer nesnesi.

        // Constructor, Redis sunucusunun ana bilgisayar adı ve port numarasını alır ve alan değişkenlerine atar.
        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        // Redis'e bağlantı kurmak için kullanılan metot.
        public void Connect() => _ConnectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");

        // Belirtilen veritabanını almak için kullanılan metot. Varsayılan olarak 1. veritabanını döner.
        public IDatabase GetDb(int db = 1) => _ConnectionMultiplexer.GetDatabase(db);
    }
}
