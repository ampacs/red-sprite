using UnityEngine;
using UnityEngine.Networking;

public class DirectionMessage : MessageBase {
    public static short MSG = MsgType.Highest + 4;
    public NetworkInstanceId playerID;
    public BlockAction.Direction direction;
}
