using UnityEngine;

public class Block : MonoBehaviour {

    public const float ACCELERATION_START = 1f;
    public const float ACCELERATION_INCREMENT = 0.1f;

    public float currentAcceleration;
    public Team team;

    private Vector2 currentMovementDirection;
    private Rigidbody2D _rigidbody;


    void Start () {
        currentAcceleration = ACCELERATION_START;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate () {
        if ((currentMovementDirection - Vector2.zero).sqrMagnitude > 1e-5) {
            _rigidbody.AddForce(currentMovementDirection * currentAcceleration, ForceMode2D.Force);
        }
    }

    public void Disable () {
        this.enabled = false;
    }

    public void RefreshAcceleration () {
        currentAcceleration += ACCELERATION_INCREMENT;
    }

    public void SetMovementDirection (Vector2 direction) {
        currentMovementDirection = direction;
    }

    public void Stop () {
        SetMovementDirection (Vector2.zero);
    }
}
