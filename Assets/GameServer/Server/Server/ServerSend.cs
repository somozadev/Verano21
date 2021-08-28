using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerSend
    {
        private static void SentTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }
        private static void SentTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SentTCPDataToAllExceptOne(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                if(i!= _exceptClient)
                    Server.clients[i].tcp.SendData(_packet);
            }
        }
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);
                SentTCPData(_toClient, _packet);
            ;}
        }
    }
}
