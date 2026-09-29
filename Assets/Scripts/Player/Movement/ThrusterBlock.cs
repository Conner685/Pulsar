using UnityEngine;

public enum ThrusterOrientation
{
    Left,
    Right,
    Forward,
    Backward
}

public class ThrusterBlock : MonoBehaviour
{
    [SerializeField]
    private float thrust;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private ThrusterOrientation orientation = ThrusterOrientation.Left;

    private float defaultSpeed = 0.1f;

    private void Awake()
    {
        if (thrust <= 0)
        {
            Debug.LogError("No thrust set");
            thrust = defaultSpeed;
        }

        if (maxSpeed <= 0)
        {
            Debug.LogError("No max speed set");
            maxSpeed = defaultSpeed;
        }
    }

    public float GetThrust()
    {
        return thrust;
    }
    

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public ThrusterOrientation GetOrientation()
    {
        return orientation;
    }
}