using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XGCClientSocketAsyncEventArgs
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 3000;
            Client m_client = new Client(ip, port);
            m_client.Start();
        }
    }
}
