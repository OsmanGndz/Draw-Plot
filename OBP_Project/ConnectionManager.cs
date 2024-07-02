using System;
using System.Data;
using System.Net.Sockets;

namespace OBP_Project
{
    public class ConnectionManager
    {
        private static ConnectionManager _instance;
        public static ConnectionManager Instance => _instance ?? (_instance = new ConnectionManager());

        public TcpClient Client { get; private set; }
        public NetworkStream Stream { get; private set; }
        public DataTable DataTable { get; set; }  // Form2'de yüklenen veriler burada tutulacak

        private ConnectionManager() { }

        public bool Connect(string serverAddress, int port)
        {
            try
            {
                Client = new TcpClient(serverAddress, port);
                Stream = Client.GetStream();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
