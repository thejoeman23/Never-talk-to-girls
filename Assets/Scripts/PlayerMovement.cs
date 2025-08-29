using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputActionReference movement;
    private Rigidbody _rb;
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 0.001f;
    Vector2 _movementDirection;
    
    Animator _animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _movementDirection = movement.action.ReadValue<Vector2>();

        SetAnimations();
        
        Camera cam = Camera.main;
        
        if (cam == null)
            return;
        
        // Convert input to Vector3
        Vector3 input = new Vector3(_movementDirection.x, 0f, _movementDirection.y);

        // Get camera directions (flatten y so movement stays on ground plane)
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Camera-relative movement
        Vector3 move = camForward * input.z + camRight * input.x;

        // Apply velocity
        _rb.linearVelocity = move * moveSpeed;

        // Rotate player to face move direction if moving
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void SetAnimations()
    {
        if (_movementDirection != Vector2.zero)
            _animator.SetBool("IsWalking", true);
        else
            _animator.SetBool("IsWalking", false);
    }
}