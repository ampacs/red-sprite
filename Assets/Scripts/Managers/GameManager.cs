using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public const int NUMBER_OF_PLAYERS = 4;

    public static GameManager instance;
    public static GameObject activeBlock;
    public static Block activeBlockComponent;

    public int scoreBlockBaseValue = 100;
    public Player[] connectedPlayers;
    public BlockAction.Direction[] playerActions;
    public bool[] playerActionsTaken;
    public Team[] playerTeams;
    public Team[] playerTeamsByID;
    public bool[] playerTeamsTaken;

    public NetworkClient client;

    private List<Vector2> blockCommandQueue;

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        blockCommandQueue = new List<Vector2>();
        //client = NetworkManager.singleton.client;
    }

    void Update () {
        if (blockCommandQueue.Count > 0) {
            activeBlockComponent.SetMovementDirection(blockCommandQueue[0]);
            blockCommandQueue.RemoveAt(0);
            activeBlockComponent.RefreshAcceleration();
        }
    }

    public BlockAction.Direction GetPlayerAction () {
        int playerAction = -1;
        for (int i = 0; i < playerActionsTaken.Length; i++)
            if (!playerActionsTaken[i]) playerAction = i;
        if (playerAction == -1)
            return BlockAction.Direction.None;
        playerActionsTaken[playerAction] = true;
        return playerActions[playerAction];
    }

    public BlockAction.Direction GetRandomPlayerAction () {
        BlockAction.Direction[] directions = {BlockAction.Direction.East, 
                                              BlockAction.Direction.North,
                                              BlockAction.Direction.South,
                                              BlockAction.Direction.West};
        int rndi = Random.Range(0, directions.Length);
        return directions[rndi];
    }

    public BlockAction.Direction GetRandomPlayerAction (BlockAction.Direction exclude) {
        BlockAction.Direction[] directions = {BlockAction.Direction.East, 
                                              BlockAction.Direction.North,
                                              BlockAction.Direction.South,
                                              BlockAction.Direction.West};
        int rndi;
        do {
            rndi = Random.Range(0, directions.Length);
        } while (directions[rndi] == exclude);
        return directions[rndi];
    }

    public void RefreshPlayerActions () {
        playerActions = BlockAction.GetRandomActions();
        playerActionsTaken = new bool[playerActions.Length];
        for (int i = 0; i < playerActionsTaken.Length; i++) {
            playerActionsTaken[i] = false;
        }
    }

    public Team GetPlayerTeam () {
        int playerTeam = -1;
        for (int i = 0; i < playerTeamsTaken.Length; i++)
            if (!playerTeamsTaken[i]) playerTeam = i;
        if (playerTeam == -1)
            return null;
        playerTeamsTaken[playerTeam] = true;
        return playerTeams[playerTeam];
    }

    public void RefreshTeamOrder() {
        int numberOfTeams = playerTeamsByID.Length;
        playerTeams = new Team[numberOfTeams];
        for (int i = 0; i < numberOfTeams; i++) {
            playerTeams[i] = playerTeamsByID[i];
        }
        for (int i = 0; i < numberOfTeams; i++) {
            int rndi = Random.Range(0, numberOfTeams - i);
            Team temp = playerTeams[rndi];
            playerTeams[rndi] = playerTeams[numberOfTeams - 1 - i];
            playerTeams[numberOfTeams - 1 - i] = temp;
        }
        playerTeamsTaken = new bool[playerTeams.Length];
        for (int i = 0; i < playerTeamsTaken.Length; i++) {
            playerTeamsTaken[i] = false;
        }
    }

    public void AddBlockCommandToQueue (BlockAction.Direction direction) {
        AddBlockCommandToQueue(BlockAction.GetActionDirection(direction));
    }

    public void AddBlockCommandToQueue (Vector2 direction) {
        blockCommandQueue.Add(direction);
    }

    public static void UpdateActiveBlock (GameObject newBlock) {
        GameManager.activeBlock = newBlock;
        GameManager.activeBlockComponent = GameManager.activeBlock.GetComponent<Block>();
    }
}
