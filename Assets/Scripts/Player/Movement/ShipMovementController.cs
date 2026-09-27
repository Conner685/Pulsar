using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovementController : MonoBehaviour
{
    private AcitveShipInput movementActions;
    private InputAction movement;

    [SerializeField]
    private Rigidbody rb;

    private void Awake()
    {
        movementActions = new AcitveShipInput();
        movement = movementActions.Movement.Movement;
        
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 v3 = movement.ReadValue<Vector3>();
        
        Debug.Log("v3: " + v3);
        
        rb.AddForce(v3, ForceMode.Impulse);
    }
}
