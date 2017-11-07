using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XGCClientSocketAsyncEventArgs
{
    class Client
    {
        public Socket m_socket;
        IPEndPoint m_endPoint;
        private SocketAsyncEventArgs m_connectSAEA;
        private SocketAsyncEventArgs m_sendSAEA;

        public Client(string ip, int port)
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse(ip);
            m_endPoint = new IPEndPoint(iPAddress, port);
            m_connectSAEA = new SocketAsyncEventArgs { RemoteEndPoint = m_endPoint };
        }

        public void Start()
        {
            m_connectSAEA.Completed += OnConnectedCompleted;
            m_socket.ConnectAsync(m_connectSAEA);


            Thread.Sleep(5000);
            Send("Hello");

            //按任意键终止服务器进程....
            //Console.WriteLine("Client : Press any key to terminate the server process....");
            Console.ReadKey();
        }

        public void OnConnectedCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) return;
            Socket socket = sender as Socket;
            string iPRemote = socket.RemoteEndPoint.ToString();

            Console.WriteLine("Client : 连接服务器 [ {0} ] 成功",iPRemote);

            SocketAsyncEventArgs receiveSAEA = new SocketAsyncEventArgs();
            byte[] receiveBuffer = new byte[1024 * 4];
            receiveSAEA.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
            receiveSAEA.Completed += OnReceiveCompleted;
            receiveSAEA.RemoteEndPoint = m_endPoint;
            socket.ReceiveAsync(receiveSAEA);
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.OperationAborted) return;

            Socket socket = sender as Socket;

            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                string ipAdress = socket.RemoteEndPoint.ToString();
                int lengthBuffer = e.BytesTransferred;
                byte[] receiveBuffer = e.Buffer;
                byte[] buffer = new byte[lengthBuffer];
                Buffer.BlockCopy(receiveBuffer, 0, buffer, 0, lengthBuffer);

                string msg = Encoding.Unicode.GetString(buffer);
                Console.WriteLine("Client : receive message[ {0} ],from Server[ {1} ]", msg, ipAdress);
                socket.ReceiveAsync(e);
            }
            else if (e.SocketError == SocketError.ConnectionReset && e.BytesTransferred == 0)
            {
                Console.WriteLine("Client: 服务器断开连接 ");
            }
            else
            {
                return;
            }
        }

        public void Send(string msg)
        {
            byte[] sendBuffer = Encoding.Unicode.GetBytes(msg);
            if (m_sendSAEA == null)
            {
                m_sendSAEA = new SocketAsyncEventArgs();
                m_sendSAEA.Completed += OnSendCompleted;
            }

            m_sendSAEA.SetBuffer(sendBuffer, 0, sendBuffer.Length);
            if (m_socket != null)
            {
                m_socket.SendAsync(m_sendSAEA);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) return;
            Socket socket = sender as Socket;
            byte[] sendBuffer = e.Buffer;

            string sendMsg = Encoding.Unicode.GetString(sendBuffer);

            Console.WriteLine("Client : Send message [ {0} ] to Serer[ {1} ]", sendMsg, socket.RemoteEndPoint.ToString());
        }

        public void DisConnect()
        {
            if (m_socket != null)
            {
                try
                {
                    m_socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException se)
                {
                }
                finally
                {
                    m_socket.Close();
                }
            }
        }
    }
}
