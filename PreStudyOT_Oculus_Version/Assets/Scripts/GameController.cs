using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using UnityEngine.SceneManagement;
public class GameController : NetworkBehaviour
{
    // Script References
    public SyncListString tasks = new SyncListString();
    public RemoteController localController;

    //// Tasks
    //public OrientationTask orientationTask;
    //public LokalisationTask lokalisationTask;

    // General Informations
    [SyncVar] public string SubjectID;

    // General Task Skript
    [SyncVar] public Condition currentCondition;
    [SyncVar] public GameState currentState = GameState.Initializing;
    [SyncVar] public bool showFixationCross;
    [SyncVar] public GameObject currentTarget;

    public static bool isConnected;


    void OnEnable()
    {
        EventManager.EventCue += CueCalledEvent;
    }
    void OnDisable()
    {
        EventManager.EventCue -= CueCalledEvent;
    }


    private void Awake()
    {
        currentState = GameState.Initializing;
        DontDestroyOnLoad(this.gameObject); // TODO: ENSURE THAT ONLY ONE INSTANCE EXISTS!
        getLocalController();
        OnAwake();

    }
    void OnAwake()
    {
        currentCondition = Condition.None;
    }
    //void DoNotDestroyOnLoad()
    //{
    //     GameController[] GCList = FindObjectsOfType<GameController>();
    //    if (GCList.Length>1)
    //    {
    //        print("multiple scripts found(" + FindObjectsOfType<GameController>().Length + "), destroy the additional");
    //        for (int i = GCList.Length; i > 0; i--)
    //        {
    //            Destroy(GCList[i]);
    //        }
    //    }

    //    DontDestroyOnLoad(this.gameObject);
    //}

    public RemoteController getLocalController()
    {
        foreach (RemoteController controller in GameObject.FindObjectsOfType<RemoteController>())
        {
            if (controller.isLocalPlayer)
            {
                localController = controller;
                return controller;
            }
        }
        return null;
    }

    private void Update()
    {
        if (localController == null || !localController.isLocalPlayer)
        {
            getLocalController();
        }
        if (currentState == GameState.Initializing)
        {
            currentState = GameState.MainMenu_EnterSubjectID;
        }
    }

    public void ShowFixationCross(bool show)
    {
        if (localController != null)
        {
            //localControl.CmdShowFixationCross(show);
        }
    }



   public void StartTutorial(GameState gameState)
    {
        localController.CmdSetGameState(gameState);
        switch (gameState)
        {
            case GameState.Task_Orientation:
                localController.CmdChangeScene(CompleteSceneName("OT"));
                break;
            case GameState.Task_Lokalisation:
                localController.CmdChangeScene(CompleteSceneName("LT"));
                break;
        }
    }

    [Server]
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
    public static string CompleteSceneName(string part)
    {
        return NameFromIndex(sceneIndexFromName(part));
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
