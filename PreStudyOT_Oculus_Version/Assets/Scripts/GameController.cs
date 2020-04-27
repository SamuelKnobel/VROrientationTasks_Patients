﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // Script References
    public TargetSpawner spawner;

    // Tasks
    public OrientationTask orientationTask;
    public LokalisationTask lokalisationTask;

    // Environment Elemtent References
    public GameObject FixationCross;

    // General Informations
    public static string SubjectID;


    // General Task Skript
    public static Condition currentCondition;
    public static GameState currentState;

    public static bool isConnected;


    public static GameObject currentTarget;


    // Task 1
    //Timer CountdownTimerTask1;

    // Task 2
    public static List<GameObject> Targets = new List<GameObject>();
    //public bool b_StartTask2;
    //Timer CountdownTimerTask2;


    void OnEnable()
    {
        //EventManager.ColliderInteractionEvent += StartCountdownTimerTask1;
        //EventManager.ColliderInteractionEvent += StartTimerTask2; ;
        EventManager.TriggerEvent += TriggerCalledEvent;
        EventManager.CueEvent += CueCalledEvent;
    }
    void OnDisable()
    {
        //EventManager.ColliderInteractionEvent -= StartCountdownTimerTask1;
        //EventManager.ColliderInteractionEvent -= StartTimerTask2; ;
        EventManager.TriggerEvent -= TriggerCalledEvent;
        EventManager.CueEvent -= CueCalledEvent;
    }


    private void Awake()
    {
        currentState = GameState.Initializing;
        // TODO: make  things dependent from Type of HMD -->Use ENUM
        DefineHMD();
        DoNotDestroyOnLoad();
        OnAwake();

    }
    void OnAwake()
    {
        spawner = FindObjectOfType<TargetSpawner>();
        currentCondition = Condition.None;
        //CountdownTimerTask2 = gameObject.AddComponent<Timer>();
        //CountdownTimerTask2.AddTimerFinishedEventListener(spawner.SpawnTarget);
    }
    void DoNotDestroyOnLoad()
    {
        if (FindObjectOfType<GameController>() != null)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            print("multiple scripts found(" + FindObjectsOfType<GameController>().Length + "), destroy the additional");
            Destroy(this.gameObject);
        }
    }


    private void Update()
    {
        if (currentState == GameState.Initializing && !isConnected)
        {
            isConnected = TryToConnect();
            currentState = GameState.MainMenu_EnterSubjectID;
        }
    }
    
    bool TryToConnect()
    {
        // Netwerk and Connection Settings
        return true;
    }

   public void StartTask(GameState gameState)
    {
        if (gameState == GameState.Task_Orientation)
        {
            currentState = GameState.Task_Orientation;
            orientationTask = gameObject.AddComponent<OrientationTask>();
        }
        else if (gameState == GameState.Task_Lokalisation)
        {
            currentState = GameState.Task_Lokalisation;
            lokalisationTask = gameObject.AddComponent<LokalisationTask>();
            Feedback.AddTextToBottom("To Be Implemented", false);
        }



        //FixationCross.SetActive(true);
    }

    //public void Restart()
    //{
    //    currentState = GameState.MainMenu_EnterSubjectID;
    //    currentTarget = null;
    //    currentCondition = Condition.None;
    //    //EventManager.ColliderInteractionEvent -= StartTimerTask2; 
    //    //EventManager.ColliderInteractionEvent += StartTimerTask2
    //    ;
    //    GameObject[] tars = GameObject.FindGameObjectsWithTag("Target");
    //    for (int i = tars.Length-1; i >=0 ; i--)
    //    {
    //        Destroy(tars[i].gameObject);
    //    }
    //    foreach (GameObject item in Targets)
    //    {
    //        Destroy(item);
    //    }
    //    Targets.Clear();
    //}

    void TriggerCalledEvent()
    {
        Feedback.AddTextToBottom("Trigger Called",true);
    }

    void CueCalledEvent(float CueType)
    {
        if (currentTarget != null)
        {
            currentTarget.GetComponent<Target>().GiveClue((int)CueType);
            Feedback.AddTextToSide("CueType:" + CueType + " for Object position: " + currentTarget.transform.position, true);
        }
        else
            Feedback.AddTextToSide("No TargetDefined", true);
    }


    /// <summary>
    /// Converts Angles in Cartesian Coordinates
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="hor_left"></param>
    /// <param name="hor_right"></param>
    /// <param name="ver_top"></param>
    /// <param name="ver_bot"></param>
    /// <returns></returns>
    public static Vector3 SpherToCart(TargetSpace targetSpace, TargetPosition targetPosition)
    {
        float radius = 0;
        float angelPhi = 0;
        float angelTheta= 0;

        float hor_left = ConfigurationUtils.HorizontalAngleLeft;
        float hor_right = ConfigurationUtils.HorizontalAngleRight;
        float ver_top = ConfigurationUtils.VerticalAngleTop;
        float ver_bot = ConfigurationUtils.VerticalAngleBottom;
        switch (targetSpace)
        {
            case TargetSpace.NearSpace:
                //radius = ConfigurationUtils.RadiusNearspace;
                radius =.55f;
                Debug.LogWarning("Radius Near Space Hardcoded");
                break;
            case TargetSpace.FarSpace:
                radius = ConfigurationUtils.RadiusFarspace;
                break;
        }
        switch (targetPosition)
        {
            case TargetPosition.left:
                angelPhi = hor_left;//  y in Unity entspricht höhe, x entspricht  links/rechts, z entspricht der tiefe
                break;
            case TargetPosition.right:
                angelPhi = hor_right;
                break;
        }

        angelTheta = UnityEngine.Random.Range(ver_bot, ver_top) + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angelTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }
    public static Vector3 SpherToCart(TargetSpace targetSpace, float anglePhi, float angleTheta)
    {
        float radius = 0;


        switch (targetSpace)
        {
            case TargetSpace.NearSpace:
                radius = ConfigurationUtils.RadiusNearspace;
                break;
            case TargetSpace.FarSpace:
                radius = ConfigurationUtils.RadiusFarspace;
                break;
        }

        angleTheta = angleTheta + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angleTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }

    //public static Vector3 CartToSpher(Vector3 position)
    //{

    //    float anglePhi=0;
    //    float angleTheta = 0;
    //    float radius = 0;




    //    return new Vector3(anglePhi, angleTheta, radius);
    //}

    void DefineHMD()
    {
        string hardware;

        //string model = UnityEngine.VR.VRDevice.model != null ? UnityEngine.VR.VRDevice.model : "";
        string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";
        if (model.IndexOf("Vive") >= 0)
        {
            hardware = "htc_vive";
        }
        else
        {
            hardware = "Others";
        }

        //print(UnityEngine.XR.XRDevice.model);

    }

}
