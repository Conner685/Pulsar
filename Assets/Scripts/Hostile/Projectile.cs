using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private Vector3 direction;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            Vector3 toPlayer
                = playerObj.transform.position - transform.position;
            toPlayer.y = 0f;
            direction = toPlayer.sqrMagnitude > 0.0001f
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