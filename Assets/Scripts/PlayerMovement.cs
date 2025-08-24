using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputActionReference movement;
    private Rigidbody _rb;
    
    [SerializeField] private float moveSpeed = 5f;
    Vector2 _movementDirection;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => _rb = GetComponent<Rigidbody>();

    // Update is called once per frame
    void Update()
    {
        _movementDirection = movement.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_movementDirection.x, 0f, _movementDirection.y).normalized * moveSpeed * Time.fixedDeltaTime;
    }
}
