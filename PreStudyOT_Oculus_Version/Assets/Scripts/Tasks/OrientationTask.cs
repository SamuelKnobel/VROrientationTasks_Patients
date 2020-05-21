using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;

public class OrientationTask : NetworkBehaviour
{
    OVRInput.Button B_X;
    OVRInput.Button B_Y;
    OVRInput.Button B_A;
    OVRInput.Button B_B;
    OVRInput.Button B_HandTrigger_R;
    OVRInput.Button B_Menu;
    [SyncVar] public int currentTargetNbr = 0;
    [SyncVar] public int currentRoundNumber = 0;
    [SyncVar] public int currentSessionNumber = 0;
    public int[] currentCueOrder;

    public int maxTargetNbr = 0;
    public int maxRoundNumber = 0;
    public int maxSessionNumber = 0;

    public SyncListInt NumTargetsPerRound = new SyncListInt();
    //Mirror doesn't support multidimensional arrays
    public SyncListInt OrderCues1 = new SyncListInt();//first session
    public SyncListInt OrderCues2 = new SyncListInt();//second session
    public SyncListInt OrderCues3 = new SyncListInt();//third session
    public SyncListInt OrderCues4 = new SyncListInt();//fourth session

    private int[] OrderCues(int i)
    {
        int[] result = new int[4];
        switch (i)
        {
            case 0: OrderCues1.CopyTo(result, 0); break;
            case 1: OrderCues2.CopyTo(result, 0); break;
            case 2: OrderCues3.CopyTo(result, 0); break;
            case 3: OrderCues4.CopyTo(result, 0); break;
        }
        return result;
    }
    [SyncVar] public bool TaskReady = false;
    [SyncVar] public bool TaskFinished = false;


    // Environment Elemtent References
    public GameObject FixationCross;
    public GameObject TargetContainer;
    [SerializeField] GameObject TargetPrefab;

    //Script References
    private GameController gameController;
    private RemoteController localController;


    public int localNumOfSessions = 0;
    public int[] localNumTargetsPerRound = new int[4];
    //private int[][] localOrderCues = new int[][] { new int[4], new int[4], new int[4], new int[4] };
    public int[] localOrderCues1 = new int[4];
    public int[] localOrderCues2 = new int[4];
    public int[] localOrderCues3 = new int[4];
    public int[] localOrderCues4 = new int[4];
    private int[] localOrderCues(int i)
    {
        switch (i)
        {
            case 0: return localOrderCues1;
            case 1: return localOrderCues2;
            case 2: return localOrderCues3;
            case 3: return localOrderCues4;
        }
        return null;
    }

    void OnEnable()
    {
        EventManager.EventTargetShot += TargetShot;
        EventManager.EventDefineNewTarget += DefineNextTarget;
        EventManager.EventStartSeachring += ShowNextTarget;
    }
    void OnDisable()
    {
        EventManager.EventDefineNewTarget -= DefineNextTarget;
        EventManager.EventStartSeachring -= ShowNextTarget;
        EventManager.EventTargetShot -= TargetShot;

    }

    void Start()
    {

        NumTargetsPerRound.Callback += OnTaskSetupUpdated;
        OrderCues1.Callback += OnTaskSetupUpdated;
        OrderCues2.Callback += OnTaskSetupUpdated;
        OrderCues3.Callback += OnTaskSetupUpdated;
        OrderCues4.Callback += OnTaskSetupUpdated;
        OnTaskSetupUpdated();
        gameController = GameObject.FindObjectOfType<GameController>();
        localController = gameController.getLocalController();
        localController.CmdSetGameState(GameState.Task_Orientation_Tutorial);

        B_X = OVRInput.Button.Three;
        B_Y = OVRInput.Button.Four;
        B_A = OVRInput.Button.One;
        B_B = OVRInput.Button.Two;
        B_HandTrigger_R = OVRInput.Button.SecondaryHandTrigger;
        B_Menu = OVRInput.Button.Start;

        vh = Screen.height / 100f;
        vw = Screen.width / 100f;
        centerRect = new Rect(25 * vw, 30 * vh, 50 * vw, 60 * vh);
        activeWindow = centerRect;

    }

    #region syncTaskSetup
    void OnTaskSetupUpdated(SyncListInt.Operation op, int index, int oldItem, int newItem)
    {
        OnTaskSetupUpdated();
    }
    //Callback to fill local arrays with values when the synced lists get updates from the server
    void OnTaskSetupUpdated()
    {
        localNumOfSessions = NumTargetsPerRound.Count;
        localNumTargetsPerRound = new int[4];
        //localOrderCues = new int[][] { new int[4], new int[4], new int[4], new int[4] };
        NumTargetsPerRound.CopyTo(localNumTargetsPerRound, 0);
        OrderCues1.CopyTo(localOrderCues1, 0);
        OrderCues2.CopyTo(localOrderCues2, 0);
        OrderCues3.CopyTo(localOrderCues3, 0);
        OrderCues4.CopyTo(localOrderCues4, 0);

        maxTargetNbr = localNumTargetsPerRound[currentSessionNumber];
        maxRoundNumber = 4;
        maxSessionNumber = NumTargetsPerRound.Count - 1;
    }
    #endregion

    void Update()
    {
        if (gameController== null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }
        if (!OVRInput.Get(B_HandTrigger_R))
        {
            if (OVRInput.GetDown(B_X)/*|| Input.GetKeyDown(KeyCode.Alpha1)*/)
                SpawnTarget_OrientationTask(0,-80,true);
            if (OVRInput.GetDown(B_Y)/* || Input.GetKeyDown(KeyCode.Alpha2)*/)
                SpawnTarget_OrientationTask(0, -40, true);
            if (OVRInput.GetDown(B_A) /*|| Input.GetKeyDown(KeyCode.Alpha3)*/)
                SpawnTarget_OrientationTask(0, 40, true);
            if (OVRInput.GetDown(B_B)/* || Input.GetKeyDown(KeyCode.Alpha4)*/)
                SpawnTarget_OrientationTask(0, 80, true);
        }
        if (OVRInput.Get(B_HandTrigger_R) /*|| Input.GetKey(KeyCode.G)*/)
        {
            if (gameController.currentTarget == null)
                SpawnTarget_OrientationTask(0, -80, true);

            if (OVRInput.GetDown(B_X)/* || Input.GetKeyDown(KeyCode.Q)*/)
                EventManager.CallCueEvent(0);

            if (OVRInput.GetDown(B_Y)/* || Input.GetKeyDown(KeyCode.W)*/)
                EventManager.CallCueEvent(1);

            if (OVRInput.GetDown(B_A)/* || Input.GetKeyDown(KeyCode.E)*/)
                EventManager.CallCueEvent(2);

            if (OVRInput.GetDown(B_B) /*|| Input.GetKeyDown(KeyCode.R)*/)
                EventManager.CallCueEvent(3);

        }
        
        // TODO!
        if (OVRInput.GetDown(B_Menu) || Input.GetKeyDown(KeyCode.T))
        {
            FixationCross.SetActive(!FixationCross.activeSelf);
            FixationCross.GetComponent<FixationCross>().TimeSeen = 0;
            FixationCross.GetComponent<FixationCross>().isSeen = false;
        }

        if (TaskReady)
        {
            gameController.currentState = GameState.Task_Orientation_Task;
        }

    }

    public void SpawnTarget_OrientationTask(int condition, float angle, bool moving)
    {
        if (isServer)
        {

            foreach (var t in FindObjectsOfType<Target>())
            {
                NetworkServer.Destroy(t.gameObject);
            }
            if (gameController.currentTarget != null)
                NetworkServer.Destroy(gameController.currentTarget);

            gameController.currentCondition = (Condition)condition;

            GameObject TargetContainer = GameObject.Find("GameControll/Targets");
            TargetContainer.transform.position = Camera.main.transform.position;

            GameObject instance = Instantiate(TargetPrefab, TargetContainer.transform);

            instance.GetComponent<Target>().defineConfiguration(angle, moving);
            instance.GetComponent<Target>().GiveClue(condition);
            NetworkServer.Spawn(instance);
            gameController.currentTarget = instance;
        }
        else
        {
            foreach (var t in FindObjectsOfType<Target>())
            {
                localController.CmdDestroy(t.gameObject);
            }
            if (gameController.currentTarget != null)
                localController.CmdDestroy(gameController.currentTarget);

            localController.CmdSetCondition((Condition)condition);
            localController.CmdSpawnTarget(condition, angle, moving);
        }
    }

    [Server]
    public void StartTask()
    {
        //ToDo: sync all variables which are updated here
        Debug.Log("StartTask");

        currentCueOrder = OrderCues(currentSessionNumber);
        gameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
        print((Condition)currentCueOrder[currentRoundNumber]);
        maxSessionNumber = NumTargetsPerRound.Count;
        maxRoundNumber = 4;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        currentTargetNbr++;
        FixationCross.SetActive(true);
        TaskReady = true;
        ShowNextTarget();
    }
    void ShowNextTarget()
    {
        SpawnTarget_OrientationTask((int)gameController.currentCondition, getRandomAngle(), true);
    }


    void TargetShot(GameObject shotObject)
    {
        if (shotObject != null)
        {
            if (shotObject.tag == "Target")
            {
                shotObject.tag = "Untagged";
                shotObject.GetComponent<Target>().deathTimer.Run();
                shotObject.GetComponent<Rigidbody>().useGravity = true;
                if (gameController.currentState == GameState.Task_Orientation_Task)
                {
                    localController.CmdCallDefineNewTargetEvent();
                }
            }
        }
    }
    // TODO

    // called if the Target is Hit
    [Server]
    public void DefineNextTarget()
    {
        if (currentTargetNbr >= maxTargetNbr)
        {
            currentTargetNbr = 0;
            currentRoundNumber++;
        }
        if (currentRoundNumber >=maxRoundNumber)
        {
            currentTargetNbr = 0;
            currentRoundNumber = 0;
            currentSessionNumber++;

        }
        if (currentSessionNumber >= maxSessionNumber)
        {
            //Feedback.AddTextToButton("Game Ends",true);
            Debug.Log("Game Ends");
           
        }
        else
        {
            currentCueOrder = OrderCues(currentSessionNumber);
            maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
            gameController.currentCondition= (Condition)currentCueOrder[currentRoundNumber]; 
            FixationCross.SetActive(true);
            currentTargetNbr++;
        }
        //ToDo: when should this be called?
        // TODO have to be replaced with the Fixation Cross logic as discussed
        ShowNextTarget();
    }

    float getRandomAngle()
    {
        int pos = Random.Range(0, 4);
        float spawnAngle = 0;
        if (pos == 0)
            spawnAngle = -80f;
        else if (pos == 1)
            spawnAngle = -40f;
        else if (pos == 2)
            spawnAngle = 40f;
        else if (pos == 3)
            spawnAngle = 80f;
        return spawnAngle;
    }

    #region HUD

    private float vh, vw;
    private Rect centerRect;
    private Rect activeWindow;

    public enum GuiMode { Tutorial, SetUpTask, Task, None };
    public GuiMode currentGUI;



    private void OnGUI()
    {
        switch (currentGUI)
        {
            case GuiMode.Tutorial:
                activeWindow = GUI.Window(0, activeWindow, guiTutorial, "Enter Subject ID"); ;
                break;
            case GuiMode.SetUpTask:
                activeWindow = GUI.Window(1, activeWindow, guiSetUpTask, "Task Setup"); ;
                break;
            case GuiMode.Task:
                activeWindow = GUI.Window(2, activeWindow, guiTask, "Task"); ;
                break;
            default:
                break;
        }
    }



    bool AudioOn = false;
    bool VibrationOn = false;
    float angle = 0;
    int cueType = -1;
    bool moving = true;
    private void guiTutorial(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Define Position of Next Target");
        GUILayout.BeginHorizontal();

        angle = GUILayout.HorizontalSlider(angle, -80, 80);

        GUILayout.Label(angle.ToString());
        GUILayout.EndHorizontal();

        AudioOn = GUILayout.Toggle(AudioOn, "Audio");
        VibrationOn = GUILayout.Toggle(VibrationOn, "Vibration");
        moving = GUILayout.Toggle(moving, "Movement");

        if (!AudioOn & !VibrationOn)
            cueType = 1;
        if (AudioOn & !VibrationOn)
            cueType = 2;
        if (!AudioOn & VibrationOn)
            cueType = 3;
        if (AudioOn & VibrationOn)
            cueType = 4;

        if (GUILayout.Button("Spawn"))
        {
            if (angle < -60)
                angle = -80;
            if (angle >= -60 & angle <= 0)
                angle = -40;
            if (angle > 60)
                angle = 80;
            if (angle <= 60 & angle > 0)
                angle = 40;
            //gameController.currentTarget = SpawnTarget_OrientationTask(cueType, angle, moving);
            SpawnTarget_OrientationTask(cueType, angle, moving);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Set Up Task"))
        {
            currentGUI = GuiMode.SetUpTask;
            localController.CmdDestroyCurrentTarget();
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }


    private Vector2 scrollposition;

    private void guiSetUpTask(int windowID)
    {
        GUIStyle nobreak = new GUIStyle(GUI.skin.label);
        nobreak.wordWrap = false;
        scrollposition = GUILayout.BeginScrollView(scrollposition);
        GUILayout.BeginVertical();
        GUILayout.Label("Wählen Sie die Anzahl der Sessionen (1 Session = 4 Runden)");
        GUILayout.BeginHorizontal();
        localNumOfSessions = (int)GUILayout.HorizontalSlider((float)localNumOfSessions, 0, 4);
        GUILayout.Label(localNumOfSessions.ToString());
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.Label("Anzahl Objekte pro Runde je Session");
        GUILayout.BeginHorizontal();
        for (int i = 0; i < localNumOfSessions; i++)
        {
            int.TryParse(Regex.Replace(GUILayout.TextField(localNumTargetsPerRound[i].ToString()), "[^0-9]", ""), out localNumTargetsPerRound[i]);
            GUILayout.Space(7);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.Label("Reihenfolge der Cues je Session: z.B: 3 2 4 1");

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("None", nobreak);
        GUILayout.Label("Audio", nobreak);
        GUILayout.Label("Tactile", nobreak);
        GUILayout.Label("Combined", nobreak);
        GUILayout.EndVertical();
        for (int i = 0; i < localNumOfSessions; i++)
        {
            GUILayout.BeginVertical();
            //orderCuesString[i] = GUILayout.TextField(orderCuesString[i]);
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                localOrderCues(i)[j] = GUILayout.SelectionGrid(localOrderCues(i)[j], new string[] { "", "", "", "" }, 1);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Start Task"))
        {
            // if (TranslateEntrys(nbObPerRound, orderCuesString))
            //{
            currentGUI = GuiMode.Task;
            localController.CmdSyncTaskSetupOT(localNumOfSessions, localNumTargetsPerRound, localOrderCues1, localOrderCues2, localOrderCues3, localOrderCues4);
            localController.CmdStartTaskOT();
            //}
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    private void guiTask(int windowID)
    {
        scrollposition = GUILayout.BeginScrollView(scrollposition);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aktuelle Session:");
        GUILayout.Label((currentSessionNumber + 1).ToString() + " / " + maxSessionNumber.ToString());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aktuelle Runde:  ");
        GUILayout.Label((currentRoundNumber + 1).ToString() + " / " + maxRoundNumber.ToString());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aktuelles Target:");
        GUILayout.Label(currentTargetNbr.ToString() + " / " + maxTargetNbr.ToString());
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.Label("Session - Übersicht:");
        for (int i = 0; i < maxSessionNumber; i++)
        {
            string order = (i + 1).ToString() + ". Session:  ";
            order += NumTargetsPerRound[i].ToString() + " Obj./R.,  Cues: ";
            foreach (int type in OrderCues(i))
            {
                switch (type)
                {
                    case 0: order += "None     "; break;
                    case 1: order += "Audio    "; break;
                    case 2: order += "Tactile  "; break;
                    case 3: order += "Combinded"; break;
                }
                order += " ";
            }
            GUILayout.Label(order);
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }
    #endregion
}
