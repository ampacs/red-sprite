using UnityEngine;
using UnityEngine.Networking;

public class NetworkMessages : MonoBehaviour {
    public static short CLIENT_SCENE_LOADED = MsgType.Highest + 1;
    public static short CLIENT_SCENE_UNLOADED = MsgType.Highest + 2;
    public static short CLIENT_ACTIVE_SCENE_CHANGED = MsgType.Highest + 3;
    public static short SERVER_RECEIVED_DIRECTION_CHANGE = MsgType.Highest + 5;
}
