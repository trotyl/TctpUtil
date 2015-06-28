using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TctpUtil
{
    public class TctpClient
    {
        private Socket m_socket;
        public Action<string> OnReceive;

        public TctpClient() : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) { }

        public TctpClient(Socket socket)
        {
            m_socket = socket;
            var receiveTask = new Task(m_readLoop);
            receiveTask.Start();
        }

        public void InitLocal()
        {
            m_socket.Connect(new IPEndPoint(IPAddress.Broadcast, 5683));
        }

        public void Connect(IPEndPoint endPoint)
        {
            m_socket.Connect(endPoint);
        }

        public void Send(string jsonMessage)
        {
            var bytesBuffer = Encoding.UTF8.GetBytes(jsonMessage);
            var sentCount = 0;
            var totalCount = bytesBuffer.Length;
            while (sentCount < totalCount)
            {
                sentCount += m_socket.Send(bytesBuffer);
            }
        }

        private void m_readLoop() 
        {
            var stringBuffer = new StringBuilder();
            var bytesBuffer = new byte[2048];
            while (true)
            {
                var readLength = m_socket.Receive(bytesBuffer);
                var readString = Encoding.UTF8.GetString(bytesBuffer, 0, readLength);
                stringBuffer.Append(readString);
                checkBuffer(stringBuffer);
            }
        }

        private void checkBuffer(StringBuilder stringBuffer)
        {
            var bufferString = stringBuffer.ToString();
            var endIndex = bufferString.IndexOf('}');
            if (endIndex > 0)
            {
                var jsonMessage = bufferString.Substring(0, endIndex + 1);
                if (OnReceive != null)
                {
                    OnReceive(jsonMessage);
                }
                stringBuffer.Remove(0, endIndex + 1);
            }
        }
    }
}
