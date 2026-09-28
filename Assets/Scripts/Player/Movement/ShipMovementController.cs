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

    private Vector3 curSpeed;
    private float curRotSpeed;

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
        curRotSpeed = rotInput * rotationalThrust * Time.fixedDeltaTime;
        curRotSpeed = Mathf.Clamp(curRotSpeed, -maxRotationalSpeed, maxRotationalSpeed);
        
        float activeRot = rb.rotation.eulerAngles.y + curRotSpeed;
        Quaternion targetRot = Quaternion.Euler(0f, activeRot, 0f);
        
        rb.MoveRotation(targetRot);

        Vector3 moveInput = movement.ReadValue<Vector3>();
        Vector3 moveDirection = targetRot * moveInput;

        curSpeed = moveDirection * thrust;
        curSpeed *= Time.fixedDeltaTime;
        curSpeed = Vector3.ClampMagnitude(curSpeed, maxSpeed);
        rb.AddForce(curSpeed, ForceMode.Force);
    }
}