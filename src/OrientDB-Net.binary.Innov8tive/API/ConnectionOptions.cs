using Orient.Client;

namespace OrientDB_Net.binary.Innov8tive.API
{
    public class ConnectionOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public ODatabaseType DatabaseType { get; set; }
    }
}
