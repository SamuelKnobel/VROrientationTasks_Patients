using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationTask : MonoBehaviour
{
    public OVRInput.Button B_X;
    public OVRInput.Button B_Y;
    public OVRInput.Button B_A;
    public OVRInput.Button B_B;
    public OVRInput.Button B_HandTrigger_R;
    public OVRInput.Button B_Menu;

    public float timerToShowStartTaskButton;

    public int currentTargetNbr = 0;
    public int currentRoundNumber = 0;
    public int[] currentCueOrder;
    public int currentSessionNumber = 0;

    public int maxTargetNbr = 0;
    public int maxRoundNumber = 0;
    public int maxSessionNumber = 0;

    public int[] NumTargetsPerRound;
    public List<int[]> OrderCues;

    void OnEnable()
    {
        EventManager.DefineNewTargetEvent += DefineNextTarget;
        EventManager.StartSeachringEvent += ShowNextTarget;
    }
    void OnDisable()
    {
        EventManager.DefineNewTargetEvent -= DefineNextTarget;
        EventManager.StartSeachringEvent -= ShowNextTarget;

    }



    // Start is called before the first frame update
    void Start()
    {
        GameController.currentState = GameState.Task_Orientation_Tutorial;

        Feedback.AddTextToButton("Hold MenuButton for Activating Task Start Button", true);

        Feedback.AddTextToButton("Press X for first SpawnPostion", false);
        Feedback.AddTextToButton("Press Y for second SpawnPostion", false);
        Feedback.AddTextToButton("Press A for third SpawnPostion", false);
        Feedback.AddTextToButton("Press B for forth SpawnPostion", false);

        Feedback.AddTextToButton("Hold R - HandTrigger and Press X for CueType: Nothing", false);
        Feedback.AddTextToButton("Hold R - HandTrigger and Press Y for CueType: Audio", false);
        Feedback.AddTextToButton("Hold R - HandTrigger and Press A for CueType: Tactile", false);
        Feedback.AddTextToButton("Hold R - HandTrigger and Press B for CueType: Combined", false);
        Feedback.AddTextToButton("Press MenuButton for Activating FixationCross", false);


        B_X = OVRInput.Button.Three;
        B_Y = OVRInput.Button.Four;
        B_A =OVRInput.Button.One;
        B_B = OVRInput.Button.Two;
        B_HandTrigger_R = OVRInput.Button.SecondaryHandTrigger;
        B_Menu = OVRInput.Button.Start;
    }

    // Update is called once per frame
    void Update()
    {
        if (!OVRInput.Get(B_HandTrigger_R))
        {
            if (OVRInput.GetDown(B_X)|| Input.GetKeyDown(KeyCode.Alpha1))
                SpawnObjectAtPosition(0,-80);
            if (OVRInput.GetDown(B_Y) || Input.GetKeyDown(KeyCode.Alpha2))
                SpawnObjectAtPosition(0, -40);
            if (OVRInput.GetDown(B_A) || Input.GetKeyDown(KeyCode.Alpha3))
                SpawnObjectAtPosition(0, 40);
            if (OVRInput.GetDown(B_B) || Input.GetKeyDown(KeyCode.Alpha4))
                SpawnObjectAtPosition(0, 80);
        }
        if (OVRInput.Get(B_HandTrigger_R) || Input.GetKey(KeyCode.G))
        {
            if (GameController.currentTarget == null)
                SpawnObjectAtPosition(0, -80);

            if (OVRInput.GetDown(B_X) || Input.GetKeyDown(KeyCode.Q))
                EventManager.CallCueEvent(0);

            if (OVRInput.GetDown(B_Y) || Input.GetKeyDown(KeyCode.W))
                EventManager.CallCueEvent(1);

            if (OVRInput.GetDown(B_A) || Input.GetKeyDown(KeyCode.E))
                EventManager.CallCueEvent(2);

            if (OVRInput.GetDown(B_B) || Input.GetKeyDown(KeyCode.R))
                EventManager.CallCueEvent(3);

        }
        if (OVRInput.GetDown(B_Menu) || Input.GetKeyDown(KeyCode.T))
        {
            FindObjectOfType<GameController>().FixationCross.SetActive(!FindObjectOfType<GameController>().FixationCross.activeSelf);
        }
        if (OVRInput.Get(B_Menu) || Input.GetKey(KeyCode.M))
        {
            if (GameController.currentState == GameState.Task_Orientation_Tutorial)
            {
                timerToShowStartTaskButton += Time.deltaTime;
                if (timerToShowStartTaskButton > 2)
                {
                    FindObjectOfType<HUD>().startTask1.gameObject.SetActive(true);
                }
            }

        }
        if (OVRInput.GetUp(B_Menu) || Input.GetKeyUp(KeyCode.M))
        {
             timerToShowStartTaskButton = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            EventManager.CallTargetShotEvent(GameController.currentTarget);

        }


    }

    public void SpawnObjectAtPosition(int cueType, float angle)
    {
       Feedback.AddTextToBottom("SpawnObject At Angle:" + angle, true);
       TargetSpawner.SpawnTarget(cueType, angle);
    }

    public bool StartTask(int[] numTargetsPerRound, List<int[]> orderCues)
    {
        Feedback.AddTextToButton("GameStarts", true);

        Debug.Log("StartTask");
        NumTargetsPerRound = new int[numTargetsPerRound.Length];
        NumTargetsPerRound = numTargetsPerRound;
        OrderCues = orderCues;

        if (NumTargetsPerRound.Length != OrderCues.Count)
        {
            Debug.LogError("INVALD INPUT");
            return false;
        }
        currentCueOrder = OrderCues[currentSessionNumber];
        Condition condition = (Condition)currentCueOrder[currentRoundNumber];
        GameController.currentCondition = condition;
        maxSessionNumber = OrderCues.Count;
        maxRoundNumber = orderCues[0].Length;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        currentTargetNbr++;
        return true;
    }
    void ShowNextTarget()
    {
        SpawnObjectAtPosition((int)GameController.currentCondition, getRandomAngle());
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
            Feedback.AddTextToButton("Game Ends",true);
            Debug.Log("Game Ends");
           
        }
        else
        {
            maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
            currentCueOrder = OrderCues[currentSessionNumber];
            Condition condition = (Condition)currentCueOrder[currentRoundNumber];
            GameController.currentCondition = condition;
            FindObjectOfType<GameController>().FixationCross.SetActive(true);
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


}
