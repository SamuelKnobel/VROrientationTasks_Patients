//using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class OrientationTask : MonoBehaviour
{
     //OVRInput.Button B_X;
     //OVRInput.Button B_Y;
     //OVRInput.Button B_A;
     //OVRInput.Button B_B;
     //OVRInput.Button B_HandTrigger_R;
     //OVRInput.Button B_Menu;

    public float timerToShowStartTaskButton;

    public int currentTargetNbr = 0;
    public int currentRoundNumber = 0;
    public int[] currentCueOrder;
    public int currentSessionNumber = 0;

    public int maxTargetNbr = 0;
    public int maxRoundNumber = 0;
    public int maxSessionNumber = 0;

    public int[] NumTargetsPerRound;
    public List<int[]> OrderCues= new List<int[]>();
    public bool TaskReady = false;

    // Environment Elemtent References
    public GameObject FixationCross;
    public GameObject TargetContainer;
    [SerializeField] GameObject TargetPrefab;

    void OnEnable()
    {
        EventManager.TargetShotEvent += TargetShot;

        EventManager.DefineNewTargetEvent += DefineNextTarget;
        EventManager.StartSeachringEvent += ShowNextTarget;
    }
    void OnDisable()
    {
        EventManager.DefineNewTargetEvent -= DefineNextTarget;
        EventManager.StartSeachringEvent -= ShowNextTarget;
        EventManager.TargetShotEvent -= TargetShot;

    }

    void Start()
    {
        GameController.recording = true;
        GameController.SavePath = Application.streamingAssetsPath + "/Output/" + GameController.SubjectID+"/";
        FindObjectOfType<GameController>().orientationTask = this;
        GameController.currentState = GameState.Task_Orientation_Tutorial;

      
        //B_X = OVRInput.Button.Three;
        //B_Y = OVRInput.Button.Four;
        //B_A =OVRInput.Button.One;
        //B_B = OVRInput.Button.Two;
        //B_HandTrigger_R = OVRInput.Button.SecondaryHandTrigger;
        //B_Menu = OVRInput.Button.Start;
    }

    void Update()
    {
        //if (!OVRInput.Get(B_HandTrigger_R))
        //{
        //    if (OVRInput.GetDown(B_X)/*|| Input.GetKeyDown(KeyCode.Alpha1)*/)
        //        SpawnTarget_OrientationTask(0,-80,true);
        //    if (OVRInput.GetDown(B_Y)/* || Input.GetKeyDown(KeyCode.Alpha2)*/)
        //        SpawnTarget_OrientationTask(0, -40, true);
        //    if (OVRInput.GetDown(B_A) /*|| Input.GetKeyDown(KeyCode.Alpha3)*/)
        //        SpawnTarget_OrientationTask(0, 40, true);
        //    if (OVRInput.GetDown(B_B)/* || Input.GetKeyDown(KeyCode.Alpha4)*/)
        //        SpawnTarget_OrientationTask(0, 80, true);
        //}
        //if (OVRInput.Get(B_HandTrigger_R) /*|| Input.GetKey(KeyCode.G)*/)
        //{
        //    if (GameController.currentTarget == null)
        //        SpawnTarget_OrientationTask(0, -80, true);

        //    if (OVRInput.GetDown(B_X)/* || Input.GetKeyDown(KeyCode.Q)*/)
        //        EventManager.CallCueEvent(0);

        //    if (OVRInput.GetDown(B_Y)/* || Input.GetKeyDown(KeyCode.W)*/)
        //        EventManager.CallCueEvent(1);

        //    if (OVRInput.GetDown(B_A)/* || Input.GetKeyDown(KeyCode.E)*/)
        //        EventManager.CallCueEvent(2);

        //    if (OVRInput.GetDown(B_B) /*|| Input.GetKeyDown(KeyCode.R)*/)
        //        EventManager.CallCueEvent(3);

        //}
        if (/*OVRInput.GetDown(B_Menu) || */Input.GetKeyDown(KeyCode.T))
        {
            FixationCross.SetActive(!FixationCross.activeSelf);
            FixationCross.GetComponent<FixationCross>().TimeSeen = 0;
            FixationCross.GetComponent<FixationCross>().isSeen = false;

        }
        //if (OVRInput.Get(B_Menu) /*|| Input.GetKey(KeyCode.M)*/)
        //{
        //    if (GameController.currentState == GameState.Task_Orientation_Tutorial)
        //    {
        //        timerToShowStartTaskButton += Time.deltaTime;
        //        if (timerToShowStartTaskButton > 2)
        //        {
        //            //FindObjectOfType<HUD_Main>().startTask1.gameObject.SetActive(true);
        //        }
        //    }

        //}
        //if (OVRInput.GetUp(B_Menu) || Input.GetKeyUp(KeyCode.M))
        //{
        //     timerToShowStartTaskButton = 0;
        //}
        if (TaskReady)
        {
            GameController.currentState = GameState.Task_Orientation_Task;
        }

    }

    public GameObject SpawnTarget_OrientationTask(int condition, float angle, bool moving)
    {
        foreach (var t in FindObjectsOfType<Target>())
        {
            //Destroy(t.gameObject);
            t.gameObject.SetActive(false);
        }
        if (GameController.currentTarget != null)
        {
            GameController.currentTarget.SetActive(false);
            //Destroy(GameController.currentTarget);
            GameController.currentTarget = null;
        }

        GameController.currentCondition = (Condition)condition;

        TargetContainer.transform.position = Camera.main.transform.position;

        GameObject NewTarget = Instantiate(TargetPrefab);
        NewTarget.GetComponent<Target>().defineConfiguration(angle, moving);

        NewTarget.transform.SetParent(TargetContainer.transform, false);
        NewTarget.GetComponent<Target>().GiveClue(condition);

        return NewTarget;
    }
  
    public void StartTask()
    {
        Debug.Log("StartTask");

        if (NumTargetsPerRound.Length != OrderCues.Count)
        {
            Debug.Log(NumTargetsPerRound.Length);
            Debug.Log(OrderCues.Count);
            Debug.LogError("INVALD INPUT");
            TaskReady= false;
        }
        currentCueOrder = OrderCues[currentSessionNumber];
        GameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
        maxSessionNumber = OrderCues.Count;
        maxRoundNumber = OrderCues[0].Length;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        currentTargetNbr++;
        FixationCross.SetActive(true);
        TaskReady =  true;
    }
    void ShowNextTarget()
    {
        GameController.currentTarget=  SpawnTarget_OrientationTask((int)GameController.currentCondition, getRandomAngle(),true);
    }


    void TargetShot(GameObject shotObject)
    {
        if (shotObject != null)
        {
            if (shotObject.tag.Contains("Target"))
            {
                shotObject.tag = "Untagged";
                shotObject.GetComponent<Target>().hit = true;
               shotObject.GetComponent<Target>().deathTimer.Run();
                shotObject.GetComponent<Rigidbody>().useGravity = true;
                //Feedback.AddTextToBottom("Target Shot", true);
                //shotObject.GetComponent<Data_Targets_OT>()..Run();
                if (GameController.currentState == GameState.Task_Orientation_Task)
                {
                    EventManager.CallDefineNewTargetEvent();
                }
            }
        }
    }


    // called if the Target is Hit
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
            TaskReady = false;
            GameController.currentState = GameState.End;
           
        }
        else
        {
            maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
            currentCueOrder = OrderCues[currentSessionNumber];
            GameController.currentCondition= (Condition)currentCueOrder[currentRoundNumber]; 
            FixationCross.SetActive(true);
            currentTargetNbr++;
        }
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

    public enum GuiMode { Tutorial, SetUpTask,Task, None };
    public GuiMode currentGUI;



    private void OnGUI()
    {
        vh = Screen.height / 100f;
        vw = Screen.width / 100f;
        centerRect = new Rect(30 * vw, 30 * vh, 40 * vw, 60 * vh);
        activeWindow = centerRect;
        switch (currentGUI)
        {
            case GuiMode.Tutorial:
                activeWindow = GUI.Window(0, activeWindow, guiTutorial, "Enter Subject ID"); ;
                break;
            //case GuiMode.Task:
                //activeWindow = GUI.Window(2, activeWindow, guiTaskselection, "Select a Task"); ;   
            case GuiMode.SetUpTask:
                activeWindow = GUI.Window(1, activeWindow, guiSetUpTask, "Select a Task"); ;
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
       
        angle= GUILayout.HorizontalSlider(angle, -80, 80);

        GUILayout.Label(angle.ToString());
        GUILayout.EndHorizontal();

        AudioOn= GUILayout.Toggle(AudioOn, "Audio");
        VibrationOn= GUILayout.Toggle(VibrationOn, "Vibration");
        moving= GUILayout.Toggle(moving, "Movement");
       
        if (!AudioOn& ! VibrationOn)
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
            if (angle>= -60 & angle <= 0)
                angle = -40;
            if (angle > 60)
                angle = 80;
            if (angle <= 60 & angle > 0)
                angle = 40;
            GameController.currentTarget = SpawnTarget_OrientationTask(cueType, angle, moving);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Set Up Task"))
        {
            currentGUI = GuiMode.SetUpTask;
            Destroy(GameController.currentTarget);
            GameController.currentTarget = null;
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }


    public string numOfSessionsString;
    public int numOfSessionsInt = 0;
    public string[] nbObPerRound = new string[0];
    public string[] orderCuesString = new string[0];
    private void guiSetUpTask(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Wählen sie die Anzahl Sessionen (1 Session = 4 Runden) ,    (0 - 4)");
        numOfSessionsString = GUILayout.TextField(numOfSessionsString);

        bool valid = int.TryParse(numOfSessionsString, out numOfSessionsInt);

        if (!valid)
            numOfSessionsString = "";
        else
            if (nbObPerRound.Length != numOfSessionsInt)
            {
                nbObPerRound = new string[numOfSessionsInt];
                orderCuesString = new string[numOfSessionsInt];
            }

        GUILayout.Label("Anzahl Objekte pro Runde je Session");
        GUILayout.BeginHorizontal();
        for (int i = 0; i < numOfSessionsInt; i++)
        {
            nbObPerRound[i] = GUILayout.TextField(nbObPerRound[i]);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Reihenfolge der Cues je Session: z.B: 3 2 4 1");
        GUILayout.BeginHorizontal();
        for (int i = 0; i < numOfSessionsInt; i++)
        {
            orderCuesString[i] = GUILayout.TextField(orderCuesString[i]);
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Start Task"))
        {
            if (TranslateEntrys(nbObPerRound, orderCuesString))
            {
                currentGUI = GuiMode.Task;
                StartTask();
            }
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    bool TranslateEntrys(string[] perRound, string[] cueorder)
    {
        bool result = false;

        NumTargetsPerRound = new int[perRound.Length];
        OrderCues = new List<int[]>();
        for (int i = 0; i < NumTargetsPerRound.Length; i++)
        {
           result=  int.TryParse(perRound[i], out NumTargetsPerRound[i]);
            if (result == false)
            {
                return result;
            }
        } 
        for (int i = 0; i < cueorder.Length; i++)
        {
            string[] SplitString = cueorder[i].Split(' ');
            int[] orderSession = new int[4];
            int sum = 0;
            for (int j = 0; j < SplitString.Length; j++)
            {
                result = int.TryParse(SplitString[j], out orderSession[j]);
                sum += orderSession[j];
                if (result == false)
                {
                    return result;
                }
            }
            if (sum !=10)
            {
                return false;
            }
            OrderCues.Add(orderSession);
        }
        return result;
    }


    #endregion
}
