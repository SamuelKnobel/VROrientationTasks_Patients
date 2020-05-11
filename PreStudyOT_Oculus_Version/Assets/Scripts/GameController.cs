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



    //public static List<GameObject> Targets = new List<GameObject>();



    void OnEnable()
    {

        //EventManager.TriggerEvent += TriggerCalledEvent;
        EventManager.CueEvent += CueCalledEvent;
    }
    void OnDisable()
    {

        //EventManager.TriggerEvent -= TriggerCalledEvent;
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

    private void Start()
    {
        FindObjectOfType<Feedback>().UIVisibility(true);
    }
    private void Update()
    {
        if (currentState == GameState.Initializing && !isConnected)
        {
            isConnected = TryToConnect();
            currentState = GameState.MainMenu_EnterSubjectID;
        }
        if (currentState == GameState.Task_Lokalisation) // Check if this is the correct Gamestate maybe its GameState.Task_Orientation_Tutorial
        {
            FixationCross fc = FindObjectOfType<FixationCross>();
            if (fc != null)
            {
                FixationCross = fc.gameObject;
            }
        }
    }
    
    bool TryToConnect()
    {
        // Netwerk and Connection Settings
        return true;
    }

   public void StartTutorial(GameState gameState)
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
    }

    public void StartTask(GameState gameState)
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }
        if (gameState == GameState.Task_Orientation)
        {
            currentState = GameState.Task_Orientation_Task;
            int[] nbTargetsPerRound = new int[2];
            nbTargetsPerRound[0] = 2;
            nbTargetsPerRound[1] = 3;
             
            List<int[]> CueOder = new List<int[]>();
            int[] orderSession1 = new int[4];
            orderSession1[0] = 3;
            orderSession1[1] = 2;
            orderSession1[2] = 1;
            orderSession1[3] = 0;
            int[] orderSession2 = new int[4];
            orderSession2[0] = 1;
            orderSession2[1] = 0;
            orderSession2[2] = 3;
            orderSession2[3] = 2;
            CueOder.Add(orderSession1);
            CueOder.Add(orderSession2);
           
            orientationTask.StartTask(nbTargetsPerRound, CueOder);  
        }
        //else if (gameState == GameState.Task_Lokalisation)
        //{
        //    currentState = GameState.Task_Lokalisation;
        //    lokalisationTask = gameObject.AddComponent<LokalisationTask>();
        //    Feedback.AddTextToBottom("To Be Implemented", false);
        //}
    }
    //void TriggerCalledEvent()
    //{
    //    Feedback.AddTextToBottom("Trigger Called",true);
    //}

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
    public static Vector3 SpherToCart(float angle)
    {
        float radius = ConfigurationUtils.Radius;
        float angelPhi = angle;//  y in Unity entspricht höhe, x entspricht  links/rechts, z entspricht der tiefe
        float angelTheta= 0;

        float hor_left = ConfigurationUtils.HorizontalAngleLeft;
        float hor_right = ConfigurationUtils.HorizontalAngleRight;
        float ver_top = ConfigurationUtils.VerticalAngleTop;
        float ver_bot = ConfigurationUtils.VerticalAngleBottom;

        //angelTheta = UnityEngine.Random.Range(ver_bot, ver_top) + 90;  // have to be turned by 90 degree!
        angelTheta = 0 + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angelTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }
    public static Vector3 SpherToCart( float anglePhi, float angleTheta)
    {
        float radius = ConfigurationUtils.Radius;

        angleTheta = angleTheta + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angleTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }

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
