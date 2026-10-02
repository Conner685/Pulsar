using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Represents a single player weapon
/// - each of the players weapon tiles will likely have this
///
/// Reference: https://www.youtube.com/watch?v=2mThTAhD16M
///
/// @author Alfredo Luzardo
/// @version 1.1
/// </summary>
public class PlayerWeapon : Weapon
{
    private float fireTimer;

    /// <summary>
    /// Trigger the weapon fire if the button is clicked
    /// </summary>
    private void Update()
    {
        fireTimer += Time.deltaTime;
    }

    public void Fire()
    {
        if (!isActiveAndEnabled ||
            fireTimer < weaponStrategy.FireRate)
        {
            return;
        }

        weaponStrategy.Fire(firePoint);
        fireTimer = 0f;
    }
}