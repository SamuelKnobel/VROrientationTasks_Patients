//using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class OrientationTask : MonoBehaviour
{
    public int currentTargetNbr = 0;
    public int currentRoundNumber = 0;
    public int[] currentCueOrder;
    public int currentSessionNumber = 0;

    public int maxTargetNbr = 0;
    public int maxRoundNumber = 0;
    public int maxSessionNumber = 0;

    public int[] NumTargetsPerRound;
    public List<int[]> OrderCues = new List<int[]>();
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
        GameController.SavePath = Application.streamingAssetsPath + "/Output/" + GameController.SubjectID + "/";
        FindObjectOfType<GameController>().orientationTask = this;
        GameController.currentState = GameState.Task_Orientation_Tutorial;
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.T))
        {
            FixationCross.SetActive(!FixationCross.activeSelf);
            FixationCross.GetComponent<FixationCross>().TimeSeen = 0;
            FixationCross.GetComponent<FixationCross>().isSeen = false;
        }

        if (TaskReady)
        {
            GameController.currentState = GameState.Task_Orientation_Task;
            TaskReady = false;
        }

    }

    public GameObject SpawnTarget_OrientationTask(int condition, float angle, bool moving, bool audioTest)
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
        if (audioTest)
        {
            NewTarget.GetComponent<SpriteRenderer>().sprite = null;
        }
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
            TaskReady = false;
        }
        currentCueOrder = OrderCues[currentSessionNumber];
        GameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
        maxSessionNumber = OrderCues.Count;
        maxRoundNumber = OrderCues[0].Length;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        currentTargetNbr++;
        FixationCross.SetActive(true);
        TaskReady = true;
    }
    void ShowNextTarget()
    {
        GameController.currentTarget = SpawnTarget_OrientationTask((int)GameController.currentCondition, getRandomAngle(), true,false);
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
        if (currentRoundNumber >= maxRoundNumber)
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
            GameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
            FixationCross.SetActive(true);
            currentTargetNbr++;
        }
    }

    float getRandomAngle()
    {
        int pos = Random.Range(0, 4);
        float spawnAngle = 0;
        if (pos == 0)
            spawnAngle = -70f;
        else if (pos == 1)
            spawnAngle = -30f;
        else if (pos == 2)
            spawnAngle = 30f;
        else if (pos == 3)
            spawnAngle = 70f;
        return spawnAngle;
    }

   
}
