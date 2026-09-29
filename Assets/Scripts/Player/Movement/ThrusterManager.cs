using System.Collections.Generic;
using UnityEngine;

public struct DirectionThrust
{
    public ThrusterOrientation orientation;
    public float thrust;
    public float maxSpeed;
}

public class ThrusterManager : MonoBehaviour
{
    [SerializeField]
    private List<ThrusterBlock> thrusters;

    private DirectionThrust[] directedThrust;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        directedThrust = new DirectionThrust[4];
        CalculateThrusters();
    }

    private void CalculateThrusters()
    {
        foreach (ThrusterBlock thruster in thrusters)
        {
            directedThrust[(int)thruster.GetOrientation()].thrust
                += thruster.GetThrust();
            directedThrust[(int)thruster.GetOrientation()].maxSpeed
                += thruster.GetMaxSpeed();
        }
    }

    public DirectionThrust GetDirectionThrust(ThrusterOrientation orientation)
    {
        return directedThrust[(int)orientation];
    }
}