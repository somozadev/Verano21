using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected as nº({_fromClient}) by username: {_username}");
            if(_clientIdCheck!= _fromClient)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID({_clientIdCheck}!!!");
            }
            //TODO : send player into game
        }
    }
}
