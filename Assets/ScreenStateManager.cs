using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScreenStateManager : NetworkBehaviour {

    public GameObject ServerMatchScreen;
    public GameObject ServerTitleScreen;
    public GameObject ClientMatchScreen;
    public GameObject ClientTitleScreen;

    public Text[] WaitingScreenWaitingForPlayer;
    public Text[] WaitingScreenPlayerReady;


    public enum ServerScreenState {
        Match, Waiting, Title, GameOver, None
    }

    public enum ClientScreenState {
        Title, Finder, Match, GameOver, None
    }

    public static ScreenStateManager instance;

    [SyncVar]
    public ServerScreenState previousServerState;
    [SyncVar]
    public ServerScreenState currentServerState;
    public ClientScreenState previousClientState;
    public ClientScreenState currentClientState;
    public bool startMatch;
    public Text[] scoreMeters;
    public Text timer;

    void Awake () {
        /* /
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);/* */
    }

    void Start () {
        if (isServer) {
            currentServerState = ServerScreenState.Waiting;
            currentClientState = ClientScreenState.None;
            //EnableServerScreen(currentServerState);
        } else {
            currentClientState = ClientScreenState.Title;
            //EnableClientScreen(currentClientState);
        }
        startMatch = false;
    }

    void Update () {
        if (isServer) {
            if (currentServerState != previousServerState) {
                EnableServerScreen(currentServerState);
            }
            if (currentServerState == ServerScreenState.Waiting) {
                startMatch = true;
                for (int i = 0; i < GameManager.instance.connectedPlayers.Count; i++) {
                    if (GameManager.instance.connectedPlayers[i].client.isConnected) {
                        WaitingScreenWaitingForPlayer[i].gameObject.SetActive(false);
                        WaitingScreenPlayerReady[i].gameObject.SetActive(true);
                    } else {
                        startMatch = false;
                    }
                }
                if (startMatch) {
                    Debug.Log("Start Match!");
                    currentServerState = ServerScreenState.Match;
                }
            } else if (currentServerState == ServerScreenState.Match) {
                if (GameManager.instance.matchFinished) {

                }
            }
            previousServerState = currentServerState;
        } else {
            //currentClientState = ClientScreenState.Title;
            if (currentClientState != previousClientState) {
                EnableClientScreen(currentClientState);
            }
            previousClientState = currentClientState;
        }
    }

    public void EnableServerScreen (ServerScreenState state) {
        //ServerMatchScreen.SetActive(false);
        //ServerTitleScreen.SetActive(false);
        switch (state) {
            case ServerScreenState.Match:
                ServerMatchScreen.SetActive(true);
                break;
            case ServerScreenState.Waiting:
                ServerTitleScreen.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void EnableClientScreen (ClientScreenState state) {
        //ClientMatchScreen.SetActive(false);
        //ClientTitleScreen.SetActive(false);
        switch (state) {
            case ClientScreenState.Match:
                ClientMatchScreen.SetActive(true);
                break;
            case ClientScreenState.Title:
                ClientTitleScreen.SetActive(true);
                break;
            default:
                break;
        }
    }
}
