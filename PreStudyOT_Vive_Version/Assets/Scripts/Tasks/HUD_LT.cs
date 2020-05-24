using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class HUD_LT : MonoBehaviour
{
    private float vh, vw;
    private Rect centerRect;
    private Rect activeWindow;
    private Vector2 scrollposition;

    //Script References
    private GameController gameController;
    private LokalisationTask LT;


    [SerializeField] int localNumOfSessions = 0;
    [SerializeField] int[] localNumTargetsPerRound = new int[4];
    [SerializeField] int[] localOrderCues1 = new int[4];
    [SerializeField] int[] localOrderCues2 = new int[4];
    [SerializeField] int[] localOrderCues3 = new int[4];
    [SerializeField] int[] localOrderCues4 = new int[4];
    [SerializeField]    int[] localOrderCues(int i)
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

    public enum GuiMode { Tutorial, SetUpTask, Task, None };
    public GuiMode currentGUI;


    private void Start()
    {
        LT = GetComponent<LokalisationTask>();
        gameController = FindObjectOfType<GameController>();
        currentGUI = GuiMode.Tutorial;
        vh = Screen.height / 100f;
        vw = Screen.width / 100f;
        centerRect = new Rect(25 * vw, 30 * vh, 50 * vw, 60 * vh);
        activeWindow = centerRect;
    }

    // Update is called once per frame
    void Update()
    {
        if (LT == null)
        {
            LT = GetComponent<LokalisationTask>();
        }
        // TODO
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            print("Saving");
            FindObjectOfType<DataHandler>().writeToFile();
        }
    }


    private void OnGUI()
    {
        switch (currentGUI)
        {
            case GuiMode.Tutorial:
                activeWindow = GUI.Window(0, activeWindow, guiTutorial, "Tutorial"); ;
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
        if (GameController.currentState == GameState.End)
        {
            currentGUI = GuiMode.None;
            activeWindow = GUI.Window(3, activeWindow, guiSaveAndClose, "Save and Close"); ;
        }

        ShowRecordingState();
        guiOverview();
    }



    public bool AudioOn = false;
    public bool VibrationOn = false;
    public int position = 0;
    public int cueType = -1;
    public bool moving = true;
    public bool audioTest = true;

    private void guiTutorial(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Define Position of Next Target (0 bis 5)");
        GUILayout.BeginHorizontal();

        position = Mathf.RoundToInt(GUILayout.HorizontalSlider(position, 0, 5));

        GUILayout.Label(position.ToString());
        GUILayout.EndHorizontal();

        audioTest = GUILayout.Toggle(audioTest, "Hör Test");
        if (audioTest)
        {
            AudioOn = true;
            VibrationOn = false;
            moving = false;
            GUILayout.Space(66);
        }
        else
        {
            AudioOn = GUILayout.Toggle(AudioOn, "Audio");
            VibrationOn = GUILayout.Toggle(VibrationOn, "Vibration");
            moving = GUILayout.Toggle(moving, "Movement");
        }

        if (!AudioOn & !VibrationOn)
            cueType = 0;
        if (AudioOn & !VibrationOn)
            cueType = 1;
        if (!AudioOn & VibrationOn)
            cueType = 2;
        if (AudioOn & VibrationOn)
            cueType = 3;

        if (GUILayout.Button("Spawn"))
        {
            LT.SpawnTargets_LokalizationTask(cueType, position, audioTest);
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
            if (TransferEntrys())
            {
                currentGUI = GuiMode.Task;
                LT.StartTask();
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }
    bool TransferEntrys()
    {
        LT.OrderCues = new List<int[]>();
        LT.NumTargetsPerRound = new int[localNumOfSessions];
        for (int i = 0; i < localNumTargetsPerRound.Length; i++)
        {
            if (localNumTargetsPerRound[i] != 0)
            {
                LT.NumTargetsPerRound[i] = localNumTargetsPerRound[i];
            }
        }
        switch (localNumOfSessions)
        {
            case 1:
                LT.OrderCues.Add(localOrderCues(0));
                break;
            case 2:
                LT.OrderCues.Add(localOrderCues(0));
                LT.OrderCues.Add(localOrderCues(1));
                break;
            case 3:
                LT.OrderCues.Add(localOrderCues(0));
                LT.OrderCues.Add(localOrderCues(1));
                LT.OrderCues.Add(localOrderCues(2));
                break;
            case 4:
                LT.OrderCues.Add(localOrderCues(0));
                LT.OrderCues.Add(localOrderCues(1));
                LT.OrderCues.Add(localOrderCues(2));
                LT.OrderCues.Add(localOrderCues(3));
                break;
            default:
                Debug.LogError("max Number of Sessions  = 4");

                return false;
        }
        return true;
    }

    private void guiTask(int windowID)
    {
        scrollposition = GUILayout.BeginScrollView(scrollposition);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aktuelle Session:");
        GUILayout.Label((LT.currentSessionNumber + 1).ToString() + " / " + LT.maxSessionNumber.ToString());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aktuelle Runde:  ");
        GUILayout.Label((LT.currentRoundNumber + 1).ToString() + " / " + LT.maxRoundNumber.ToString());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aktuelles Target:");
        GUILayout.Label(LT.currentTargetNbr.ToString() + " / " + LT.maxTargetNbr.ToString());
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.Label("Session - Übersicht:");
        for (int i = 0; i < LT.maxSessionNumber; i++)
        {
            string order = (i + 1).ToString() + ". Session:  ";
            order += LT.NumTargetsPerRound[i].ToString() + " Obj./R.,  Cues: ";
            foreach (int type in LT.OrderCues[i])
            {
                switch (type)
                {
                    case 0: order += "None     "; break;
                    case 1: order += "Audio    "; break;
                    case 2: order += "Tactile  "; break;
                    case 3: order += "Combined"; break;
                }
                order += " ";
            }
            GUILayout.Label(order);
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    #region Recording
    void ShowRecordingState()
    {
        GUILayout.BeginArea(new Rect(0, 90 * vh, 50 * vw, 5 * vh));
        GUILayout.BeginHorizontal();
        GUILayout.Label("RecordingState: " + GameController.recording);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    #endregion

    #region SaveAndClose
    bool saving;
    bool saved;
    string ButtonText;

    private void Quit()
    {
        Debug.Log("Close Application");
        Application.Quit();

    }
    void guiSaveAndClose(int window)
    {
        if (!saving)
        {
            if (GUI.Button(new Rect(10 * vw, 10 * vh, 20 * vw, 30), "Speichern"))
            {
                ButtonText = "Quit";
                FindObjectOfType<DataHandler>().writeToFile();
                saving = true;
            }
        }
        else if (!saved)
        {
            if (GUI.Button(new Rect(10 * vw, 10 * vh, 20 * vw, 30), ButtonText))
            {
                saved = true;
                Invoke("Quit", 5);
            }
        }
        else
        {
            ButtonText = "Schliesst nach Speichern automatisch";
            GUI.Label(new Rect(10 * vw, 10 * vh, 30 * vw, 30), ButtonText);
        }
    }

    #endregion

    #region Overview
    void guiOverview()
    {
        GUILayout.BeginArea(new Rect(0, 95 * vh, 100 * vw, 5 * vh));
        GUILayout.BeginHorizontal();
        GUILayout.Label("State: " + GameController.currentState.ToString());
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    #endregion


}
