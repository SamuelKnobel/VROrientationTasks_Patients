using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetConfiguration
{
    protected float Size;
    protected float Speed;
    protected TargetPosition Position;
    protected float Radius;
    protected int direction;

    public TargetConfiguration(TargetPosition position)
    {
        Position = position;
    }

    public void Initialize()
    {
        Speed = ConfigurationUtils.TargetSpeed;
        Size = ConfigurationUtils.TargetSizeFar;
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
