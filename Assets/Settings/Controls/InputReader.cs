using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "InputReader",
    menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, IShipActions
{
    private Controls _controls;
    public Action<Vector2> ShipMoveEvent;

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Ship.SetCallbacks(this);
        }

        _controls.Ship.Enable();
    }

    private void OnDisable()
    {
        _controls.Ship.Disable();
    }

    public void OnMoveShip(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveShip");
        ShipMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
}