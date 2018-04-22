using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScreenStateManager : NetworkBehaviour {

    public enum ServerScreenState {
        Match, Waiting, Title, GameOver, None
    }

    public enum ClientScreenState {
        Title, Finder, Match, GameOver, None
    }

    public static ScreenStateManager instance;

    [SyncVar]
    public ServerScreenState currentServerState;
    public ClientScreenState currentClientState;
    public Text[] scoreMeters;
    public Text timer;

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        if (isServer) {
            currentServerState = ServerScreenState.Title;
            currentClientState = ClientScreenState.None;
        } else {
            currentClientState = ClientScreenState.Title;
        }
    }

    void Update () {
        
    }
}
