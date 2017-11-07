using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// https://msdn.microsoft.com/zh-cn/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx
/// </summary>
namespace XGCServerSocketAsyncEventArgs
{
    /// <summary>
    /// Represents a collection of reusable SocketAsyncEventArgs objects.  
    /// 表示可重用的SocketAsyncEventArgs对象的集合。
    /// </summary>
    public class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> m_pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
