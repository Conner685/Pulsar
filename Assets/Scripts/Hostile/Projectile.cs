using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float MinAimSquared = 0.0001f;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float lifetime = 5f;

    private Vector3 direction;

    private void Start()
    {
        // TODO: Replace the player object lookup per projectile with a shared player reference
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            Vector3 toPlayer
                = playerObj.transform.position - transform.position;
            /*
             * makes the projectile move horizontally on the XZ plane,
             * ignoring any height difference between its spawn point and the player.
             */
            toPlayer.y = 0f;
            direction = toPlayer.sqrMagnitude > MinAimSquared
                ? toPlayer.normalized
                : transform.forward;
        }
        else
        {
            direction = transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO: damage the player here
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}