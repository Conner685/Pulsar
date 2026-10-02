using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovementController : MonoBehaviour
{
    private ActiveShipInput movementActions;
    private InputAction movement;
    private InputAction rotation;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float coreThrust;

    [SerializeField]
    private float rotationalThrust;
    
    [SerializeField]
    private ThrusterManager thrusterManager;

    private void Awake()
    {
        movementActions = new ActiveShipInput();
        movement = movementActions.Movement.Movement;
        rotation = movementActions.Movement.Rotation;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        if (thrusterManager == null)
        {
            thrusterManager = GetComponent<ThrusterManager>();
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

    private void FixedUpdate()
    {
        float rotInput = rotation.ReadValue<float>();
        rb.AddRelativeTorque(Vector3.up * (rotInput * rotationalThrust),
            ForceMode.Force);
        
        Vector3 moveInput = movement.ReadValue<Vector3>();
        Vector3 localForce = Vector3.zero;

        if (moveInput.z > 0)
        {
            DirectionThrust t
                = thrusterManager.GetDirectionThrust(
                    ThrusterOrientation.Forward);
            localForce.z = moveInput.z * (t.thrust + coreThrust);
        }
        else if (moveInput.z < 0)
        {
            DirectionThrust t =  thrusterManager.GetDirectionThrust(
                ThrusterOrientation.Backward);
            localForce.z = moveInput.z * (t.thrust + coreThrust);
        }

        if (moveInput.x > 0)
        {
            DirectionThrust t = thrusterManager.GetDirectionThrust(
                ThrusterOrientation.Right);
            localForce.x = moveInput.x * (t.thrust + coreThrust);
        }
        else if (moveInput.x < 0)
        {
            DirectionThrust t = thrusterManager.GetDirectionThrust(
                ThrusterOrientation.Left);
            localForce.x = moveInput.x * (t.thrust + coreThrust);
        }
        
        rb.AddRelativeForce(localForce, ForceMode.Force);
        
    }
}