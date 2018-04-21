using UnityEngine;

public class Player : MonoBehaviour {

    public const int SCORE_INCREMENT = 100;

    public int score = 0;
    public Team team;

    [SerializeField]
    private Vector2 input;
    private Block block;
    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if ((input - Vector2.zero).sqrMagnitude > 1e-5) {
            GameManager.instance.AddBlockCommandToQueue(input);
        }
    }

    void AddScore (int scoreIncrement) {
        score += scoreIncrement;
    }
}
