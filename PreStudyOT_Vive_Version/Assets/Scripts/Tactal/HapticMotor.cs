using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticMotor
{
    private int id;
    public float positionDeg { get; private set; }
    public float intensity { get; set; }

    public HapticMotor(int id, float positionDeg)
    {
        this.id = id;
        this.positionDeg = positionDeg;
    }

    public void SetIntensity(float angleDeg, float maxIntensity, IntensityProfile profile, float halfVibeRange)
    {
        float deltaPos = angleDeg - positionDeg;

        if (Mathf.Abs(deltaPos) <= halfVibeRange)
        {
            switch (profile)
            {
                case IntensityProfile.CONSTANT:
                    intensity = maxIntensity;
                    break;
                case IntensityProfile.LINEAR:
                    intensity = (1 - Mathf.Abs(deltaPos) / halfVibeRange) * maxIntensity;
                    break;
                case IntensityProfile.QUADRATIC:
                    intensity = (-Mathf.Pow(Mathf.Abs(deltaPos) / halfVibeRange, 2) + 1) * maxIntensity;
                    break;
                case IntensityProfile.SINUSOIDAL:
                    intensity = (Mathf.Cos(Mathf.PI * deltaPos / halfVibeRange) + 1) / 2 * maxIntensity;
                    break;
                case IntensityProfile.TRAPEZOIDAL:
                    float slopeWidth = 0.3f; // slope width in percent of vibeHalfWidth (e.g. if slopewidth=0.1, then 10% of the half-vibration is dedicated to acceleration)
                    float a = maxIntensity / (slopeWidth * halfVibeRange); // slope parameter
                    float b = a * halfVibeRange; // offset parameter
                    if (Mathf.Abs(deltaPos / halfVibeRange) > (1 - slopeWidth))
                    {
                        if (deltaPos < 0)
                            intensity = a * deltaPos + b;
                        else
                            intensity = -a * deltaPos + b;
                    }
                    else
                    {
                        intensity = maxIntensity;
                    }
                    break;
            }
        }
        else
        {
            intensity = 0;
        }
    }
}
