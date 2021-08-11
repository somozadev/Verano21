using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {

        public static int maxPlayers { get; private set; }
        public static int port { get; private set; }

        private static TcpListener tcpListener;
         
        public void Start(int _maxPlayers, int _port)
        {
            maxPlayers = _maxPlayers;
            port = _port;
            Console.WriteLine("Starting server...");
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectionCallback), null);
            Console.WriteLine($"Server stared on port {port}.");
        }
        
        private static void TCPConnectionCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectionCallback), null);

        }


    }
}
