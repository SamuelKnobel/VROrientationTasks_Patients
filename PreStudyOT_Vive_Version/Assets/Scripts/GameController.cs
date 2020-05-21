using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour
{

    // Tasks
    public OrientationTask orientationTask;
    public LokalisationTask lokalisationTask;

    // General Informations
    public static string SubjectID = "test";

    // Paths
    public static string SavePath;


    // General Task Skript
    public static Condition currentCondition;
    public static GameState currentState;

    public static bool isConnected;
    public static GameObject currentTarget;
    public static bool recording = false;

    public static int startTime;


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
        Time.timeScale = 1;
        startTime = Mathf.RoundToInt((float)DataHandler.ConvertToTimestamp(DateTime.UtcNow));
        print(startTime);
        currentState = GameState.Initializing;
        DoNotDestroyOnLoad();
        OnAwake();

    }
    void OnAwake()
    {
        currentCondition = Condition.None;
    }
    void DoNotDestroyOnLoad()
    {
         GameController[] GCList = FindObjectsOfType<GameController>();
        if (GCList.Length>1)
        {
            print("multiple scripts found(" + FindObjectsOfType<GameController>().Length + "), destroy the additional");
            for (int i = GCList.Length; i > 0; i--)
            {
                Destroy(GCList[i]);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }


    private void Update()
    {
        if (currentState == GameState.Initializing && !isConnected)
        {
            isConnected = TryToConnect();
            currentState = GameState.MainMenu_EnterSubjectID;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            print("Saving");
            FindObjectOfType<DataHandler>().writeToFile();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentTarget != null)
            {
                currentTarget.GetComponent<Target>().hit = true;
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
            SceneManager.LoadScene(sceneIndexFromName("OT"));
        }
        else if (gameState == GameState.Task_Lokalisation)
        {
            currentState = GameState.Task_Lokalisation;
            SceneManager.LoadScene(sceneIndexFromName("LT"));
        }
    }

    public void StartTask(GameState gameState)
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }

        //else if (gameState == GameState.Task_Lokalisation)
        //{
        //    currentState = GameState.Task_Lokalisation;
        //    lokalisationTask = gameObject.AddComponent<LokalisationTask>();
        //    Feedback.AddTextToBottom("To Be Implemented", false);
        //}
    }
    void CueCalledEvent(float CueType)
    {
        if (currentTarget != null)
        {
            currentTarget.GetComponent<Target>().GiveClue((int)CueType);
        }

    }


    public static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
    public static int sceneIndexFromName(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string testedScreen = NameFromIndex(i);
            //print("sceneIndexFromName: i: " + i + " sceneName = " + testedScreen);
            if (testedScreen.Contains(sceneName))
                return i;
            if (testedScreen == sceneName)
                return i;
        }
        return -1;
    }

    #region Vector Conversion
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

    #endregion
}
