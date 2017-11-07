using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace XGCServerSocketAsyncEventArgs
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 3000;
            IPAddress ipAdress = IPAddress.Parse(ip);
            IPEndPoint ipe = new IPEndPoint(ipAdress, port);

            Server server = new Server(1024, 64);
            server.Init();
            server.Start(ipe);
        }
    }
}
