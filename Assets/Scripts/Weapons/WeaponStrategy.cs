using UnityEngine;

/// <summary>
/// Represents a WeaponStrategy
///
/// Reference: https://www.youtube.com/watch?v=2mThTAhD16M
///
/// @author Alfredo Luzardo
/// @version 1.0
/// </summary>
[CreateAssetMenu(fileName = "WeaponStrategy",
    menuName = "Scriptable Objects/WeaponStrategy")]
public abstract class WeaponStrategy : ScriptableObject
{
    [SerializeField]
    protected int damage = 10;

    [SerializeField]
    protected float fireRate = 0.5f;

    [SerializeField]
    protected float radius = 1f;

    [SerializeField]
    protected float cooldown = 1f;

    [SerializeField]
    protected float projectileSpeed = 10f;

    [SerializeField]
    protected float lifeTime = 4f;

    [SerializeField]
    protected GameObject projectilePrefab;

    // some public properties we may want to access
    public int Damage => damage;
    public float FireRate => fireRate;

    /// <summary>
    /// Abstract Fire function
    /// 
    /// Has layer so we know the layer to put projectile onto
    /// </summary>
    /// <param name="firePoint"></param>
    /// <param name="layer"></param>
    public abstract void Fire(Transform firePoint, LayerMask layer);
}