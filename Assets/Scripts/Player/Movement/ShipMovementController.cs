using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovementController : MonoBehaviour
{
    private AcitveShipInput movementActions;
    private InputAction movement;
    private InputAction rotation;

    [SerializeField]
    private Rigidbody rb;

    private void Awake()
    {
        movementActions = new AcitveShipInput();
        movement = movementActions.Movement.Movement;
        rotation = movementActions.Movement.Rotation;
        
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        movement.Enable();
        rotation.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        rotation.Disable();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float rotInput = rotation.ReadValue<float>();

        float activeRot = rb.rotation.eulerAngles.y + rotInput * 15f;
        
        Quaternion targetRot = Quaternion.Euler(0f, activeRot, 0f);
        rb.MoveRotation(targetRot);
        
        Vector3 moveInput = movement.ReadValue<Vector3>();
        
        Vector3 moveDirection = targetRot * moveInput;
        
        rb.AddForce(moveDirection, ForceMode.Impulse);
    }
}
