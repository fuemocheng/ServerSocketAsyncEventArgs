using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// https://msdn.microsoft.com/zh-cn/library/bb517542.aspx
/// </summary>
namespace XGCServerSocketAsyncEventArgs
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up 
    /// and assigned to SocketAsyncEventArgs objects for use with each 
    /// socket I/O operation.  
    /// This enables bufffers to be easily reused and guards against 
    /// fragmenting heap memory.
    /// The operations exposed on the BufferManager class are not thread safe.
    /// 
    /// 该类创建一个可以分割并分配给SocketAsyncEventArgs对象的单个大型缓冲区，用于每个套接字 I/O 操作。
    /// 这样可以轻松地重用缓冲区，并防止破坏堆内存。
    /// BufferManager类上暴露的操作不是线程安全的。
    /// </summary>
    public class BufferManager
    {
        private int m_numBytes;                 // the total number of bytes controlled by the buffer pool      由缓冲池控制的总字节数
        private byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager   缓冲区管理器维护的底层字节数组
        private Stack<int> m_freeIndexPool;     // 用来存储每个 释放的SocketAsyncEventArgs对象的数据缓冲区的偏移 的栈结构
        private int m_currentIndex;             // 相当于一个游标，判断缓冲区最后存储字节的位置
        private int m_bufferSize;               // 传入字节的大小

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// Allocates buffer space used by the buffer pool   分配缓冲池使用的缓冲区
        /// </summary>
        public void InitBuffer()
        {
            // create one big large buffer and divide that out to each SocketAsyncEventArg object
            // 创建一个大的大缓冲区，并将其分隔给每个SocketAsyncEventArg对象
            m_buffer = new byte[m_numBytes];
        }


        /// <summary>
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object
        /// <returns>true if the buffer was successfully set, else false</returns>
        /// 将缓冲池的缓冲区分配给指定的SocketAsyncEventArgs对象
        /// 如果缓冲区成功设置，则返回true，否则返回false
        /// </summary>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        /// <summary>
        /// Removes the buffer from a SocketAsyncEventArg object.  
        /// This frees the buffer back to the buffer pool
        /// 从SocketAsyncEventArg对象中删除缓冲区。
        /// 将缓冲区释放回缓冲池
        /// </summary>
        public void FreeBugffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}
