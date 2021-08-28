using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log("Msg: " + _msg);
        Debug.Log("Id: " + _myId);
        
        Client.instance.myId = _myId;

        //packet back 2 the server
        ClientSend.WelcomeReceived();
    }

}
