using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace Server
{
    class Client
    {
        public int id;
        public TCP tcp;
        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            public static int dataBufferSize = 4096;
            public readonly int id;
            private NetworkStream stream;
            private Packet receivedData; 
            private byte[] receiveBuffer; 

            public TCP(int _id)
            {
                id = _id;
            }
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();
                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiveBuffer, 0,dataBufferSize,RecieveCallback, null);
                ServerSend.Welcome(id, "Welcome to the server!");
            }
            public void SendData(Packet _packet)
            {
                try
                {
                    if(socket!=null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error sending data to player {id} via TPC: {e}");
                }
            }
            private void RecieveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if(_byteLength <=0)
                    {
                        //disconnect 
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    //Handle data
                    receivedData.Reset(HandleData(_data));

                    stream.BeginRead(receiveBuffer, 0, _byteLength, RecieveCallback, null); 


                }
                catch(Exception e)
                {
                    Console.WriteLine($"Exception recieving TCP data : {e}");
                    //disconnect
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLenght = 0;
                receivedData.SetBytes(_data);
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLenght = receivedData.ReadInt();
                    if (_packetLenght <= 0)
                        return true;
                }
                while (_packetLenght > 0 && _packetLenght <= receivedData.UnreadLength())
                {
                    byte[] _packageBytes = receivedData.ReadBytes(_packetLenght);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packageBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id,_packet);
                        }
                    });
                    _packetLenght = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLenght = receivedData.ReadInt();
                        if (_packetLenght <= 0)
                            return true;
                    }
                }

                if (_packetLenght <= 1)
                    return true;

                return false;
            }
        }
    }
}
