using UnityEngine;

public class Block : MonoBehaviour {

    public const float ACCELERATION = 10f;
    public const float VELOCITY_MAX_START = 2.5f;
    public const float VELOCITY_INCREMENT = 0.5f;

    public float velocityMax;
    public Team team;

    private Vector2 currentMovementDirection;
    private Rigidbody2D _rigidbody;


    void Start () {
        velocityMax = VELOCITY_MAX_START;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate () {
        if ((currentMovementDirection - Vector2.zero).sqrMagnitude > 1e-5) {
            _rigidbody.AddForce(currentMovementDirection * ACCELERATION, ForceMode2D.Force);
        }
        _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, velocityMax);
    }

    public void Disable () {
        this.enabled = false;
    }

    public void DisablePhysics () {
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
    }

    public void RefreshAcceleration () {
        velocityMax += VELOCITY_INCREMENT;
    }

    public void SetMovementDirection (Vector2 direction) {
        currentMovementDirection = direction;
    }

    public void Stop () {
        SetMovementDirection (Vector2.zero);
    }
}
