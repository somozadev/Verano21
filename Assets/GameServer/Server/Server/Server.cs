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

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;  


        private static TcpListener tcpListener;
         
        public static void Start(int _maxPlayers, int _port)
        {
            maxPlayers = _maxPlayers;
            port = _port;
            Console.WriteLine("Starting server...");
            InitializeServerData();
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectionCallback), null);
            Console.WriteLine($"Server stared on port {port}.");
        }
        
        private static void TCPConnectionCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectionCallback), null);

            Console.WriteLine($"Incomming connection from {_client.Client.RemoteEndPoint}...");
            for (int i = 1; i <= maxPlayers; i++)
            {
                if(clients[i].tcp.socket==null)
                {
                    //means its empty
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }
            Console.WriteLine($"{_client.Client.RemoteEndPoint} falied to connect: Server full!");

        }
           
        private static void InitializeServerData()
        {
            for(int i = 1; i <= maxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int) ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived}
            };
            Console.WriteLine("Initialized packets.");
        
        }

    }
}
