using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    //create private internal references
    private InputActions _inputActions;
    private InputAction _movement;
    private Rigidbody _rb;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float acceleration = 8f;

    [SerializeField]
    private float deceleration = 3f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>(); //get rigidbody, responsible for enabling collision with other colliders
        _inputActions = new InputActions(); //create new InputActions
        _movement = _inputActions.Player.Movement;
    }

    //called when script enabled
    private void OnEnable()
    {
        _movement.Enable();
    }

    //called when script disabled
    private void OnDisable()
    {
        _movement.Disable();
    }

    //called every physics update
    private void FixedUpdate()
    {
        Vector2 input = _movement.ReadValue<Vector2>();
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 targetVelocity = new Vector3(input.x, 0f, input.y) * moveSpeed;

        float changeRate
            = input.sqrMagnitude > 0.001f ? acceleration : deceleration;

        Vector3 currentVelocity = _rb.linearVelocity;
        currentVelocity.y = 0f;

        _rb.linearVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            changeRate * Time.fixedDeltaTime);
    }
}