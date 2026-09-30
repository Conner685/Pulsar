using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected WeaponStrategy weaponStrategy;

    [SerializeField]
    protected Transform firePoint;

    [SerializeField]
    protected int layer;

    private void OnValidate() => layer = gameObject.layer;

    /// <summary>
    /// Setter for weapon strategy
    /// </summary>
    /// <param name="strat"></param>
    private void SetWeaponStrategy(WeaponStrategy strat)
    {
        weaponStrategy = strat;
    }
}