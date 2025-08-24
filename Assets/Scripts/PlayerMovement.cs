using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputActionReference movement;
    private Rigidbody _rb;
    
    [SerializeField] private float moveSpeed = 5f;
    Vector2 _movementDirection;
    
    void Start() => _rb = GetComponent<Rigidbody>();

    void Update()
    {
        _movementDirection = movement.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Convert input into world-space movement relative to player rotation
        Vector3 move = new Vector3(_movementDirection.x, 0f, _movementDirection.y);

        // TransformDirection rotates the vector by the player's transform
        move = transform.TransformDirection(move).normalized;

        // Apply velocity (remove Time.fixedDeltaTime here — velocity is units/sec already)
        _rb.linearVelocity = move * moveSpeed;
    }
}