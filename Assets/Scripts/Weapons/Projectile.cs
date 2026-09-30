using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private GameObject hitEffectPrefab;

    private Transform _parent;

    public void SetSpeed(float input) => speed = input;
    public void SetParent(Transform parent) => _parent = parent;

    // private void Start()
    // {
    //     // Will need to do some sort of effect (e.g. muzzle flash) here later
    //     throw new NotImplementedException();
    // }

    private void Update()
    {
        transform.SetParent(null);
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

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