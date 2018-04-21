using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Custom/Team")]
public class Team : ScriptableObject {
    public short teamID;
    public Color color;
}
