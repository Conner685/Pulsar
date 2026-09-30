using System;
using UnityEngine;

/// <summary>
/// Represents a Projectile
///
/// Reference: https://www.youtube.com/watch?v=2mThTAhD16M
/// 
/// @author Alfredo Luzardo
/// @version 1.0
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private GameObject launchEffectPrefab;

    [SerializeField]
    private GameObject hitEffectPrefab;

    private Transform _parent;

    // Some useful setters
    public void SetSpeed(float input) => speed = input;
    public void SetParent(Transform parent) => _parent = parent;

    // Start Method ->
    //      will need to activate the launch effect

    /// <summary>
    /// Move the projectile
    /// </summary>
    private void Update()
    {
        transform.SetParent(null);
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    /// <summary>
    /// When it collides, activate the hit effect, damage to the other,
    /// and destroy itself
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (hitEffectPrefab != null)
        {
            // Hit effect
        }

        // Damage

        Destroy(gameObject);
    }
}