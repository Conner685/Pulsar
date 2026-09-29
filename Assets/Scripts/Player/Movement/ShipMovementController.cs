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
    private float maxSpeed;
    [SerializeField]
    private float thrust;

    [SerializeField]
    private float rotationalThrust;
    [SerializeField]
    private float maxRotationalSpeed;

    private void Awake()
    {
        movementActions = new ActiveShipInput();
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
        rb.AddRelativeTorque(Vector3.up * (rotInput * rotationalThrust), ForceMode.Force);

        if (rb.angularVelocity.magnitude > maxRotationalSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxRotationalSpeed;
        }

        Vector3 moveInput = movement.ReadValue<Vector3>();
        Vector3 moveDirection = rb.rotation * moveInput;
        
        rb.AddForce(moveDirection * thrust, ForceMode.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}