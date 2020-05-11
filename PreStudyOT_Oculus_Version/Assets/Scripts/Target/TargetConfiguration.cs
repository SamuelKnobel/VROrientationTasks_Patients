using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetConfiguration
{
    protected float Size;
    protected float Speed;
    protected TargetSide Side;
    protected float Radius;
    protected int direction;
    protected float startAngle;

    public TargetConfiguration(float angle)
    {
        TargetSide position;
        if (angle < 0)
            position = TargetSide.left;
        else
            position = TargetSide.right;
        Side = position;
        startAngle = angle;
    }

    public void Initialize()
    {
        Speed = ConfigurationUtils.TargetSpeed;
        Size = ConfigurationUtils.TargetSizeFar;
    }

    public int getDirection()
    {
        int direction;

        switch (Side)
        {
            case TargetSide.left :
                direction = 1;
                break;
            case TargetSide.right:
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
    public float getStartAngle()
    {
        return startAngle;
    }   
    public float getSize()
    {
        return Size;
    }
}
