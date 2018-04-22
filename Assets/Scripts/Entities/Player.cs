using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public const int SCORE_INCREMENT = 100;
    public static NetworkIdentity localPlayer;

    public NetworkClient client;

    [SyncVar]
    public int score = 0;
    [SyncVar]
    public int teamID;
    public Team team;
    [SyncVar]
    public BlockAction.Direction currentDirection;


    [SerializeField]
    private Vector2 input;
    private Block block;

    void Start () {
        localPlayer = GetComponent<NetworkIdentity>();
        client = NetworkManager.singleton.client;
        if (client.isConnected) {
            client.RegisterHandler(NetworkMessages.SERVER_RECEIVED_DIRECTION_CHANGE, NetworkManagerExtended.instance.ClientReceiveDirectionMessageReceptionConfirmationMessage);
        }
    }

    void Update () {
        if (!isLocalPlayer)
            return;

        //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //if ((input - Vector2.zero).sqrMagnitude > 1e-5) {
        team = GameManager.instance.playerTeamsByID[teamID];
        if (Input.GetButtonDown("Jump")) {
            Debug.Log("HERE2");
            // GameManager.instance.AddBlockCommandToQueue(input);
            SendDirectionCommand ();
            Debug.Log(score);
        }
    }
    void SendDirectionCommand () {
        if (isServer)
            return;
        
        Debug.Log("HERE1");
        DirectionMessage msg = new DirectionMessage();
        msg.playerID = localPlayer.netId;
        msg.direction = currentDirection;
        NetworkManager.singleton.client.Send(DirectionMessage.MSG, msg);
    }

    public void AddScore (int scoreIncrement) {
        score += scoreIncrement;
    }

    public void AddScore () {
        score += SCORE_INCREMENT;
    }
}
