using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TctpUtil
{
    public class TctpListener
    {
        private Socket m_socket;

        public TctpListener()
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Bind(new IPEndPoint(IPAddress.Any, 5683));
        }

        public TctpClient Accept()
        {
            return new TctpClient(m_socket.Accept());
        }
    }
}
