using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;
using System;

public enum IntensityProfile { CONSTANT, LINEAR, QUADRATIC, SINUSOIDAL, TRAPEZOIDAL }
public enum VibrationMode { CONTINOUS, SACCADIC }


public class Tactal
{
    private TactSource source;
    private long refTime;
    private long elapsedTicks;

    private List<HapticMotor> MotorList = new List<HapticMotor>();
    private int nbMotors = 6;
    private float halfVibeRangeDeg; // Half of the angular range over which a motor is active
    private float borderHalfVibeRangeDeg; // Angular range over which the border motors are active when working alone
    private int motorSpacingDeg = 20; // Angular spacing between the motors
    private float overlay = 0.1f; // How much two consecutive vibes overlay
    public IntensityProfile profile { get; set; } = IntensityProfile.TRAPEZOIDAL;
    public VibrationMode mode { get; set; } = VibrationMode.SACCADIC;
    public float maxIntensityPerc { get; set; } = 30;
    public int saccadicDelayMs { get; set; } = 200;
    public bool borderFadeOut = false; // Fading out of the intensity when the object is out of sight


    // Constructor
    public Tactal(ref TactSource source)
    {
        this.source = source;

        halfVibeRangeDeg = motorSpacingDeg * (1+overlay);
        borderHalfVibeRangeDeg = 180f - 2.5f * motorSpacingDeg;

        float motorPositionDeg;

        for (int idx = 0; idx < nbMotors; idx++)
        {
            this.source.DotPoints[idx] = 0; // Set motor intensity to 0
            motorPositionDeg = (idx - 2) * motorSpacingDeg - motorSpacingDeg / 2;
            MotorList.Add(new HapticMotor(idx, motorPositionDeg));
        }

        this.source.TimeMillis = 100; // Set the duration of a single motor vibration

        refTime = DateTime.Now.Ticks;
    }

    public void Play(float angleDeg)
    {
        for (int idx = 0; idx < nbMotors; idx++)
        {
            if (angleDeg <= MotorList[0].positionDeg && idx == 0)
            {
                if (borderFadeOut)
                    MotorList[idx].SetIntensity(angleDeg, maxIntensityPerc, IntensityProfile.LINEAR, borderHalfVibeRangeDeg); // Linear fadout from last motor position to the back of the head
                else
                    MotorList[idx].SetIntensity(angleDeg, maxIntensityPerc, IntensityProfile.CONSTANT, borderHalfVibeRangeDeg);
            }
            else if (angleDeg >= MotorList[nbMotors-1].positionDeg && idx == nbMotors - 1)
            {
                if (borderFadeOut)
                    MotorList[idx].SetIntensity(angleDeg, maxIntensityPerc, IntensityProfile.LINEAR, borderHalfVibeRangeDeg); // Linear fadout from last motor position to the back of the head
                else
                    MotorList[idx].SetIntensity(angleDeg, maxIntensityPerc, IntensityProfile.CONSTANT, borderHalfVibeRangeDeg);

            }
            else
            {
                MotorList[idx].SetIntensity(angleDeg, maxIntensityPerc, profile, halfVibeRangeDeg);
            }

            source.DotPoints[idx] = (byte)MotorList[idx].intensity;
        }

        switch (mode)
        {
            case (VibrationMode.CONTINOUS):
                source.Play();
                break;
            case (VibrationMode.SACCADIC):
                elapsedTicks = DateTime.Now.Ticks - refTime;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                if ((elapsedSpan.TotalMilliseconds) > saccadicDelayMs)
                {
                    refTime = DateTime.Now.Ticks;
                    source.Play();
                }
                break;
        }
        
    }
}
