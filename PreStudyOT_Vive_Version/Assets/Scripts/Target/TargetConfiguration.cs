using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetConfiguration
{
    protected float Size;
    protected float Speed;
    protected TargetSpace Space;
    protected TargetPosition Position;
    protected float Radius;
    protected int direction;

    protected TargetConfiguration(TargetSpace space, TargetPosition position)
    {
        Space = space;
        Position = position;
    }

    public virtual void Initialize()
    {
        Speed = ConfigurationUtils.TargetSpeed;
    }

    public int getDirection()
    {
        int direction;

        switch (Position)
        {
            case TargetPosition.left:
                direction = 1;
                break;
            case TargetPosition.right:
                direction = -1;
                break;
            default:
                direction = 0;
                break;
        }
        return direction;
    }
    public float getSpeed()
    {
        return Speed;
    }   
    public float getSize()
    {
        return Size;
    }
}

public class TargetNear : TargetConfiguration
{
    public TargetNear(TargetSpace space, TargetPosition position) : base (space, position)
    {
    }
    public override void Initialize()
    {
        base.Initialize();
        //Size = ConfigurationUtils.TargetSizeNear;
        Size = 0.1f;
        Debug.LogWarning("TargetSize Near Space Hardcoded");
        Space = TargetSpace.NearSpace;
    }
}
public class TargetFar : TargetConfiguration
{
    public TargetFar(TargetSpace space, TargetPosition position): base (space, position)
    {
    }
    public override void Initialize()
    {
        base.Initialize();
        Size = ConfigurationUtils.TargetSizeFar;
        Space = TargetSpace.FarSpace;
    }
}
