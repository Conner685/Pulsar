using System;
using UnityEngine;

/// <summary>
/// Represents a single player weapon
/// - each of the players weapon tiles will likely have this
///
/// Reference: https://www.youtube.com/watch?v=2mThTAhD16M
///
/// @author Alfredo Luzardo
/// @version 1.0
/// </summary>
public class PlayerWeapon : Weapon
{
    private float fireTimer;

    /// <summary>
    /// Trigger the weapon fire if the button is clicked
    /// </summary>
    private void Update()
    {
        // Get reference to player controls

        fireTimer += Time.deltaTime;

        // if Fire key clicked, firetimer is <= fireRate
        //      fire weapon strategy
        //      fireTimer = 0f
    }
}