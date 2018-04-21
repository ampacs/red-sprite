using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public const int NUMBER_OF_PLAYERS = 4;

    public static GameManager instance;
    public static GameObject activeBlock;
    public static Block activeBlockComponent;

    public int scoreBlockBaseValue = 100;
    public Player[] connectedPlayers;
    public BlockAction.Direction[] playerActions;

    private List<Vector2> blockCommandQueue;

    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        blockCommandQueue = new List<Vector2>();
    }

    void Update () {
        if (blockCommandQueue.Count > 0) {
            activeBlockComponent.SetMovementDirection(blockCommandQueue[0]);
            blockCommandQueue.RemoveAt(0);
            activeBlockComponent.RefreshAcceleration();
        }
    }

    public void RefreshPlayerActions () {
        playerActions = BlockAction.GetRandomActions();
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
