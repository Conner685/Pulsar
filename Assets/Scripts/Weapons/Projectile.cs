using System;
using UnityEngine;

/// <summary>
/// Represents a Projectile
///
/// Reference: https://www.youtube.com/watch?v=2mThTAhD16M
/// 
/// @author Alfredo Luzardo
/// @version 1.1
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private GameObject launchEffectPrefab;

    [SerializeField]
    private GameObject hitEffectPrefab;

    // Some useful setters
    public void SetSpeed(float input) => speed = input;

    // Start Method ->
    //      will need to activate the launch effect

    /// <summary>
    /// Move the projectile
    /// </summary>
    private void Update()
    {
        transform.SetParent(null);
        transform.position += transform.up * (speed * Time.deltaTime);
    }

    /// <summary>
    /// When it collides, activate the hit effect, damage to the other,
    /// and destroy itself
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bullets cant collide
        if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            return;
        }

        if (hitEffectPrefab != null)
        {
            Debug.Log("Hit Effect");
            // Hit effect
        }

        // Will need to deal damage here?

        Destroy(gameObject);
    }
}