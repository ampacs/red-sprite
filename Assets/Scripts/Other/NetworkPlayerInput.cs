using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayerInput : NetworkBehaviour {

    public class CustomMsgID {
        public const short customID = 200;
    };
    public class ExampleMessage : MessageBase {
        public int message;

        public ExampleMessage () {
        }
        public ExampleMessage (int msg) {
            message = msg;
        }
    }

    public bool a;
    NetworkClient _client;

    // Use this for initialization
    void Start () {
        _client = NetworkManager.singleton.client;
        if (isServer) {
            NetworkServer.RegisterHandler(200, ServerReceiveMessage );
        }
        if (_client.isConnected) {
            _client.RegisterHandler(200, ClientReceiveMessage);
        }
        Debug.Log(netId);
    }
    
    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;
        
        a = Input.GetKey(KeyCode.A);
        ExampleMessage mes = new ExampleMessage(100);
        if (a) {
            NetworkManager.singleton.client.Send(CustomMsgID.customID, mes);
        }

        if (isServer && NetworkServer.connections.Count > 1) {
            
            Debug.Log("***");
            foreach (NetworkConnection connection in NetworkServer.connections) {
                if (connection != null) {
                    Debug.Log(connection.connectionId + " " + connectionToClient.connectionId + " " + connectionToServer.connectionId);
                    if (connection.connectionId != connectionToClient.connectionId) {
                        NetworkServer.SendToClient(connection.connectionId, 200, mes);
                    }
                }
            }
        }
    }

    public void ServerReceiveMessage (NetworkMessage msg) {
        Debug.Log("Message received:");
        ExampleMessage received = msg.ReadMessage<ExampleMessage>();
        Debug.Log("Message -> " + received.message);
    }

    public void ClientReceiveMessage (NetworkMessage msg) {
        Text text = GameObject.Find("Canvas/Text").GetComponent<Text>();
        text.text = "Hello!";
    }
}
