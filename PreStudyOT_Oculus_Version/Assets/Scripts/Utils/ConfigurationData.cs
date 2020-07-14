using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Provides access to configuration data
/// </summary>
public class ConfigurationData
{
	#region Fields

    const string ConfigurationDataFileName = "ConfigurationData.csv";
    Dictionary<ConfigurationDataValueName, float> values =
        new Dictionary<ConfigurationDataValueName, float>();

	#endregion

	#region Properties
    
    /// <summary>
    ///  Defines if the Laser is used or not
    /// </summary>
    public int UseLaser
    {
        get { return (int)values[ConfigurationDataValueName.UseLaser];}
    }    
    
    /// <summary>
    /// Defines if the TargetCross is used. If turnd on, Laser is automatically turned on as well!
    /// </summary>
    public int UseTargetCross
    {
        get { return (int)values[ConfigurationDataValueName.UseTargetCross]; }
        set { values[ConfigurationDataValueName.UseTargetCross] = value; }
    }
    
    /// <summary>
    ///Defines the Time that passes after a target disapears and the next appears
    /// </summary>
    public float TimeBetweenTargets
    {
        get { return values[ConfigurationDataValueName.TimeBetweenTargets]; }
    }
    
    /// <summary>
    /// Defines the Speed of the Target
    /// </summary>
    public float TargetSpeed
    {
        get { return values[ConfigurationDataValueName.TargetSpeed]; }
    }
    
    /// <summary>
    /// Defines the Size of the Target in Near Space
    /// </summary>
    public float TargetSizeNear
    {
        get { return values[ConfigurationDataValueName.TargetSizeNear]; }
    }
    
    /// <summary>
    /// Defines the Size of the Target in Far Space
    /// </summary> 7
    public float TargetSizeFar
    {
        get { return values[ConfigurationDataValueName.TargetSizeFar]; }
    }
    
    /// <summary>
    /// Number of targets per trial. one condition has multiple Conditions
    /// </summary>
     public int NumberOfTargetsPerTrial
    {
        get { return (int)values[ConfigurationDataValueName.NumberOfTargetsPerTrial]; }
    }   
    
    /// <summary>
    ///  Number of Trials per Condition
    /// </summary>
    public int NumberOfTrials
    {
        get { return (int)values[ConfigurationDataValueName.NumberOfTrials]; }
    }    
    
    /// <summary>
    ///  Movement Radius in Farspace
    /// </summary>
    public float RadiusFarspace
    {
        get { return values[ConfigurationDataValueName.RadiusFarspace]; }
    }  
    
    /// <summary>
    ///  Movement Radius in Nearspace
    /// </summary>
    public float RadiusNearspace
    {
        get { return values[ConfigurationDataValueName.RadiusNearspace]; }
    }    
    /// <summary>
    ///  Movement Radius 
    /// </summary>
    public float Radius
    {
        get { return values[ConfigurationDataValueName.Radius]; }
    }

    /// <summary>
    ///  left border of horizontal Angle where the Targets can spawn
    /// </summary>
    public float HorizontalAngleLeft
    {
        get { return (int)values[ConfigurationDataValueName.HorizontalAngleLeft]; }
    }

    /// <summary>
    ///  right Border of horizontal Angle where the Targets can spawn
    /// </summary>
    public float HorizontalAngleRight
    {
        get { return (float)values[ConfigurationDataValueName.HorizontalAngleRight]; }
    }   
    
    /// <summary>
    ///  Top Border of vertical Angle where the Targets can spawn
    /// </summary>
    public float VerticalAngleTop
    {
        get { return (float)values[ConfigurationDataValueName.VerticalAngleTop]; }
    }

    /// <summary>
    ///  Bottom Border of vertical Angle where the Targets can spawn
    /// </summary>
    public float VerticalAngleBottom
    {
        get { return (float)values[ConfigurationDataValueName.VerticalAngleBottom]; }
    }





    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// Reads configuration data from a file. If the file
    /// read fails, the object contains default values for
    /// the configuration data
    /// </summary>
    public ConfigurationData()
    {
        GameController gameController = GameObject.FindObjectOfType<GameController>();
#if UNITY_ANDROID && !UNITY_EDITOR
        gameController.debugConfig = "Start";
#endif
        BetterStreamingAssets.Initialize();

        if (!BetterStreamingAssets.FileExists(ConfigurationDataFileName))
        {
            Debug.LogErrorFormat("Streaming asset not found: {0}", ConfigurationDataFileName);
#if UNITY_ANDROID && !UNITY_EDITOR
        gameController.debugConfig = "Streaming asset not found";
#endif
            return;
        }

        // read and save configuration data from file
        StreamReader input = null;
        string currentLine = null;

        try
        {
            // create stream reader object
            input = BetterStreamingAssets.OpenText(ConfigurationDataFileName);
            // populate values
            currentLine = input.ReadLine();
            while (currentLine != null)
            {
                string[] tokens = currentLine.Split(',');
                ConfigurationDataValueName valueName =
                    (ConfigurationDataValueName)Enum.Parse(
                        typeof(ConfigurationDataValueName), tokens[0]);
                values.Add(valueName, float.Parse(tokens[1]));
                currentLine = input.ReadLine();
#if UNITY_ANDROID && !UNITY_EDITOR
                gameController.debugConfig += " - " + tokens[0] + ": " + tokens[1];
#endif
            }
        }
        catch (Exception e)
        {
            gameController.debugConfig = e.ToString();
            // set default values if something went wrong
            Debug.Log(e);
            SetDefaultValues();
            Debug.Log(currentLine);
        }
        finally
        {
            // always close input file
            if (input != null)
            {
                input.Close();
            }
        }






        /*
        // read and save configuration data from file
        StreamReader input = null;
        string currentLine= null;
        GameController gameController = GameObject.FindObjectOfType<GameController>();
#if UNITY_ANDROID && !UNITY_EDITOR
        gameController.debugConfig = "Start";
#endif
        try
        {
            // create stream reader object
            input = File.OpenText(Path.Combine(
                Application.streamingAssetsPath, ConfigurationDataFileName));
#if UNITY_ANDROID && !UNITY_EDITOR
            gameController.debugConfig += " " + input + "  ";
#endif
            // populate values
            currentLine = input.ReadLine();
            while (currentLine != null)
            {
                string[] tokens = currentLine.Split(',');
                ConfigurationDataValueName valueName = 
                    (ConfigurationDataValueName)Enum.Parse(
                        typeof(ConfigurationDataValueName), tokens[0]);
                values.Add(valueName, float.Parse(tokens[1]));
                currentLine = input.ReadLine();
#if UNITY_ANDROID && !UNITY_EDITOR
                gameController.debugConfig += " - " + tokens[0] + ": " + tokens[1];
#endif
            }
        }
        catch (Exception e)
        {
            gameController.debugConfig = e.ToString();
            // set default values if something went wrong
            Debug.Log(e);
            SetDefaultValues();
            Debug.Log(currentLine);
        }
        finally
        {
            // always close input file
            if (input != null)
            {
                input.Close();
            }
        }
        */
    }

#endregion

    /// <summary>
    /// Sets the configuration data fields to default values
    /// csv string
    /// </summary>
    void SetDefaultValues()
    {
        Debug.LogWarning("Set Default Values!");
        values.Clear();
        values.Add(ConfigurationDataValueName.UseLaser, 0);
        values.Add(ConfigurationDataValueName.UseTargetCross, 0);
        values.Add(ConfigurationDataValueName.TimeBetweenTargets, 11);
        values.Add(ConfigurationDataValueName.TargetSpeed, 5);
        values.Add(ConfigurationDataValueName.TargetSizeNear, .2f);
        values.Add(ConfigurationDataValueName.TargetSizeFar, 2f);
        values.Add(ConfigurationDataValueName.NumberOfTargetsPerTrial, 10);
        values.Add(ConfigurationDataValueName.NumberOfTrials, 3);
        values.Add(ConfigurationDataValueName.RadiusFarspace, 10);
        values.Add(ConfigurationDataValueName.RadiusNearspace,.5f);
        values.Add(ConfigurationDataValueName.Radius,10f);
        values.Add(ConfigurationDataValueName.HorizontalAngleLeft,-90);
        values.Add(ConfigurationDataValueName.HorizontalAngleRight,90);
        values.Add(ConfigurationDataValueName.VerticalAngleTop,15);
        values.Add(ConfigurationDataValueName.VerticalAngleBottom, -15);
    }
}