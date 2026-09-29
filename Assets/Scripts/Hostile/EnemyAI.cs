using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State { Roaming, Chasing, }

    private State _currentState = State.Roaming;
    private Rigidbody _rb;
    private Vector3 _roamCenter;
    private Vector3 _roamTarget;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float roamSpeed = 2f;

    [SerializeField]
    private float chaseSpeed = 4f;

    [SerializeField]
    private float acceleration = 6f;

    [SerializeField]
    private float detectionRange = 5f;

    [SerializeField]
    private float escapeRange = 7f;

    [SerializeField]
    private float roamRadius = 4f;

    // choose first destination around the spawn point
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _roamCenter = _rb.position;
        GetRoamTarget();
    }

    // pick a random point within the roaming radius
    private void GetRoamTarget()
    {
        Vector2 offset = Random.insideUnitCircle * roamRadius;

        _roamTarget = _roamCenter +
                      new Vector3(offset.x, 0f, offset.y);
    }

    // check for state changes
    private void FixedUpdate()
    {
        UpdateState();

        if (_currentState == State.Chasing)
        {
            Chase();
        }
        else
        {
            Roam();
        }
    }

    private void UpdateState()
    {
        if (target == null)
        {
            if (_currentState == State.Chasing)
            {
                _currentState = State.Roaming;
                GetRoamTarget();
            }

            return;
        }

        Vector3 toPlayer = target.position - _rb.position;
        toPlayer.y = 0f;
        float distanceSquared = toPlayer.sqrMagnitude;

        if (_currentState == State.Roaming &&
            distanceSquared <= detectionRange * detectionRange)
        {
            _currentState = State.Chasing;
        }
        else if (_currentState == State.Chasing &&
                 distanceSquared > escapeRange * escapeRange)
        {
            _currentState = State.Roaming;
            GetRoamTarget();
        }
    }

    private void Roam()
    {
        Vector3 direction = _roamTarget - _rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.25f)
        {
            GetRoamTarget();
            direction = _roamTarget - _rb.position;
        }

        Move(direction, roamSpeed);
    }

    private void Chase()
    {
        Vector3 direction = target.position - _rb.position;
        Move(direction, chaseSpeed);
    }

    private void Move(Vector3 direction, float speed)
    {
        direction.y = 0f;

        Vector3 desiredVelocity =
            Vector3.ClampMagnitude(direction, 1f) * speed;

        Vector3 velocity = Vector3.MoveTowards(
            _rb.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime
        );

        velocity.y = 0f;
        _rb.linearVelocity = velocity;
    }
}