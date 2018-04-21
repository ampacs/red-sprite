using UnityEngine;

public class BlockAction : MonoBehaviour {

    public const int NUMBER_OF_ACTIONS = 4;
    public enum Direction {
        North, South, East, West, None
    }

    public static BlockAction.Direction[] GetRandomActions() {
        int[] actions = new int[NUMBER_OF_ACTIONS];
        BlockAction.Direction[] blockActions = new BlockAction.Direction[NUMBER_OF_ACTIONS];
        for (int i = 0; i < NUMBER_OF_ACTIONS; i++)
            actions[i] = i;
        for (int i = 0; i < NUMBER_OF_ACTIONS; i++) {
            int rndi = Random.Range(0, NUMBER_OF_ACTIONS - i);
            int temp = actions[rndi];
            actions[rndi] = actions[NUMBER_OF_ACTIONS - 1 - i];
            actions[NUMBER_OF_ACTIONS - 1 - i] = temp;
        }
        for (int i = 0; i < NUMBER_OF_ACTIONS; i++) {
            blockActions[i] = GetActionByIndex(actions[i]);
        }
        return blockActions;
    }

    private static BlockAction.Direction GetActionByIndex (int index) {
        BlockAction.Direction direction;
        switch (index) {
            case 0:
                direction = Direction.North;
                break;
            case 1:
                direction = Direction.South;
                break;
            case 2:
                direction = Direction.East;
                break;
            case 3:
                direction = Direction.West;
                break;
            default:
                direction = Direction.None;
                break;
        }
        return direction;
    }

    public static Vector2 GetActionDirection (Direction direction) {
        Vector2 actualDirection;
        switch (direction) {
            case Direction.North:
                actualDirection = Vector2.up;
                break;
            case Direction.South:
                actualDirection = Vector2.down;
                break;
            case Direction.East:
                actualDirection = Vector2.right;
                break;
            case Direction.West:
                actualDirection = Vector2.left;
                break;
            default:
                actualDirection = Vector2.zero;
                break;
        }
        return actualDirection;
    }
}
