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
        Vector3 moveVector3 = movement.ReadValue<Vector3>();
        
        rb.AddForce(moveVector3, ForceMode.Impulse);

        float rotInput = rotation.ReadValue<float>();
        
        float activeRot = rb.rotation.eulerAngles.y + rotInput * 15f;
        
        rb.rotation = Quaternion.Euler(0f, activeRot, 0f);
    }
}
