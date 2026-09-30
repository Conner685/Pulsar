using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 4f;
    public float acceleration = 6f;
    public float stoppingDistance = 6f; // Stop approaching once this close
    public float retreatDistance = 3f; // Back away if closer than this
    public float turnSpeed = 4f; // How quickly the ship rotates
    public float driftAmount = 0.3f;
    public float weaveFrequency = 1.5f;
    public GameObject projectile;
    public Transform firePoint;
    public float startTimeBtwShots = 1.5f;
    public float shootingRange = 8f;
    public float aimTolerance = 15f;

    private Transform player;
    private Vector3 velocity;
    private float timeBtwShots;
    private float weaveOffset;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning(
                "EnemyFollow: no object tagged 'Player' found in the scene.");
        }

        timeBtwShots = startTimeBtwShots;
        weaveOffset
            = Random.value * 10f;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;
        Vector3 dirToPlayer = distance > 0.001f
            ? toPlayer / distance
            : transform.forward;

        HandleMovement(dirToPlayer, distance);
        HandleRotation(dirToPlayer, distance);
        HandleShooting(dirToPlayer, distance);
    }

    private void HandleMovement(Vector3 dirToPlayer, float distance)
    {
        Vector3 desiredVelocity = Vector3.zero;

        if (distance > stoppingDistance)
        {
            desiredVelocity = dirToPlayer * speed; // approach
        }
        else if (distance < retreatDistance)
        {
            desiredVelocity = -dirToPlayer * speed; // back away
        }


        if (desiredVelocity != Vector3.zero && driftAmount > 0f)
        {
            Vector3 sideways = Vector3.Cross(Vector3.up, dirToPlayer);
            float weave = Mathf.Sin(Time.time * weaveFrequency + weaveOffset) *
                          driftAmount;
            desiredVelocity += sideways * (weave * speed);
            desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, speed);
        }

        velocity = Vector3.MoveTowards(velocity, desiredVelocity,
            acceleration * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    private void HandleRotation(Vector3 dirToPlayer, float distance)
    {
        Vector3 lookDir;
        if (distance <= stoppingDistance || velocity.sqrMagnitude < 0.05f)
        {
            lookDir = dirToPlayer;
        }
        else
        {
            lookDir = velocity.normalized;
        }

        if (lookDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation
            = Quaternion.LookRotation(lookDir, Vector3.up);

        float t = 1f - Mathf.Exp(-turnSpeed * Time.deltaTime);
        transform.rotation
            = Quaternion.Slerp(transform.rotation, targetRotation, t);
    }

    private void HandleShooting(Vector3 dirToPlayer, float distance)
    {
        timeBtwShots -= Time.deltaTime;

        if (timeBtwShots > 0f || projectile == null)
        {
            return;
        }

        if (distance > shootingRange)
        {
            return;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleToPlayer > aimTolerance)
        {
            return;
        }

        Transform spawn = firePoint != null ? firePoint : transform;
        Instantiate(projectile, spawn.position,
            spawn.rotation);
        timeBtwShots = startTimeBtwShots;
    }
}