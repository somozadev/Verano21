using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
public class Client : MonoBehaviour
{
    public static Client instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Start()
    {
        tcp = new TCP();
    }
    public void ConnectToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] reciveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            reciveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);

        }



        private void ConnectCallBack(IAsyncResult _result)
        {
            if (!socket.Connected)
                return;
            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(reciveBuffer, 0, dataBufferSize, RecieveCallback, null);
        }
        private void RecieveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    //disconnect 
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(reciveBuffer, _data, _byteLength);

                //Handle data

                receivedData.Reset(HandleData(_data));


                stream.BeginRead(reciveBuffer, 0, _byteLength, RecieveCallback, null);


            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception recieving TCP data : {e}");
                //disconnect
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if(socket!=null)
                {
                    stream.BeginWrite(_packet.ToArray(),0,_packet.Length(),null,null);
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Error sending data to server via TCP: {e}");
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
                        packetHandlers[_packetId](_packet);
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

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) ServerPackets.welcome,ClientHandle.Welcome},
        };
        Debug.Log("Initialized packets. ");
    }
}
