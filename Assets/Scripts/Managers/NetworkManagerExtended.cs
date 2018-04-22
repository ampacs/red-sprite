using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerExtended : NetworkManager {

    public static NetworkManagerExtended instance;

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        NetworkServer.RegisterHandler(DirectionMessage.MSG, ServerReceiveDirectionMessage);
    }

    public void ServerReceiveDirectionMessage (NetworkMessage netMsg) {
        DirectionMessage msg = netMsg.ReadMessage<DirectionMessage>();
        Debug.Log("PlayerID: " + msg.playerID + "; Direction: " + msg.direction);
        GameManager.instance.AddBlockCommandToQueue(msg.direction);
        msg.direction = GameManager.instance.GetRandomPlayerAction(msg.direction);
        msg.playerID = NetworkInstanceId.Invalid;
        NetworkServer.SendToClient(netMsg.conn.connectionId, NetworkMessages.SERVER_RECEIVED_DIRECTION_CHANGE, msg);
    }

    public void ClientReceiveDirectionMessageReceptionConfirmationMessage (NetworkMessage netMsg) {
        Debug.Log("Message Received! Changing direction...");
        Player player = Player.localPlayer.gameObject.GetComponent<Player>();
        DirectionMessage msg = netMsg.ReadMessage<DirectionMessage>();
        player.currentDirection = msg.direction;
    }

    public void StartHosting () {
        base.StartHost();
    }

    public override void OnStartClient(NetworkClient client) {
        Debug.Log("Client Started!");
        base.OnStartClient(client);
    }

    public override void OnStartHost() {
        Debug.Log("Host Started!");
        base.OnStartHost();
        GameManager.instance.RefreshPlayerActions();
        GameManager.instance.RefreshTeamOrder();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerID) {

        base.OnServerAddPlayer(conn, playerControllerID);
        
        Debug.Log("Adding Player");
        if (conn.playerControllers.Count > 0) {
            foreach (PlayerController playerController in conn.playerControllers) {
                GameObject playerObject = playerController.gameObject;
                Player player = playerObject.GetComponent<Player>();
                if (player != null && !playerController.unetView.isLocalPlayer) {
                    player.team = GameManager.instance.GetPlayerTeam();
                    Debug.Log(player.team);
                    player.teamID = player.team.teamID;
                    player.currentDirection = GameManager.instance.GetPlayerAction();
                }
            }
        }
    }
}
