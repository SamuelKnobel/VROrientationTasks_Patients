using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides utility access to configuration data
/// </summary>
public static class ConfigurationUtils
{
	#region Fields

	static ConfigurationData configurationData;

	#endregion

	#region Properties

    public static bool UseLaser
    {
        get
        {
            if (configurationData.UseLaser == 1)
                return true;
            else
            {
                configurationData.UseTargetCross = 0;
                return false;
            }
        }
    }
    public static bool UseTargetCross
    {
        get
        {
            if (configurationData.UseTargetCross == 1)
                return true;
            else
                return false;
        }
    }
    public static float TimeBetweenTargets
    {
        get { return configurationData.TimeBetweenTargets; }
    }   
    public static float TargetSpeed
    {
        get { return configurationData.TargetSpeed; }
    }    
    public static float TargetSizeNear
    {
        get { return configurationData.TargetSizeNear; }
    }
    public static float TargetSizeFar
    {
        get { return configurationData.TargetSizeFar; }
    }    
    public static float NumberOfTargetsPerTrial
    {
        get { return configurationData.NumberOfTargetsPerTrial; }
    }   
    public static float NumberOfTrials
    {
        get { return configurationData.NumberOfTrials; }
    }    
    public static float RadiusFarspace
    {
        get { return configurationData.RadiusFarspace; }
    }    
    public static float RadiusNearspace
    {
        get { return configurationData.RadiusNearspace; }
    }    
    public static float HorizontalAngleLeft
    {
        get { return configurationData.HorizontalAngleLeft; }
    }    
    public static float HorizontalAngleRight
    {
        get { return configurationData.HorizontalAngleRight; }
    }    
    public static float VerticalAngleTop
    {
        get { return configurationData.VerticalAngleTop; }
    }    
    public static float VerticalAngleBottom
    {
        get { return configurationData.VerticalAngleBottom;
        }
    }


    

    #endregion



    #region Public methods

    /// <summary>
    /// Initializes the configuration data by creating the ConfigurationData object 
    /// </summary>
    public static void Initialize()
	{
        configurationData = new ConfigurationData();
	}

	#endregion
}
