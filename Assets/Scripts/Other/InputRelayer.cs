using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;

public class InputRelayer : MonoBehaviour {

    public string serverURL = "http://10.128.167.160:8080";

    protected Socket socket = null;

    // Use this for initialization
    void Start () {
        socket = IO.Socket(serverURL);
        socket.On(Socket.EVENT_CONNECT, () => OnConnect());
        socket.On("hi", (data) => OnGreeting(data));
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    void OnConnect () {
        Debug.Log("Device connected!");
    }

    void OnGreeting (object data) {
        Debug.Log(data);
        socket.Emit("hello");
        //socket.Disconnect();
    }
}
