using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected WeaponStrategy weaponStrategy;

    [SerializeField]
    protected Transform firePoint;

    [SerializeField]
    protected int layer;

    private void OnValidate() => layer = gameObject.layer;

    private void SetWeaponStrategy(WeaponStrategy strat)
    {
        weaponStrategy = strat;
    }
}