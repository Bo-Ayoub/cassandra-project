namespace cassandra_app.Services
{
    public class CassandraSettings
    {
        public string ContactPoints { get; set; }
        public int Port { get; set; }
        public string Keyspace { get; set; }
        public string LocalDataCenter { get; set; }
    }
}
