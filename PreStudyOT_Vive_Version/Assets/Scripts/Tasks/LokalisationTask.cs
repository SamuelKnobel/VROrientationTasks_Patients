using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LokalisationTask : MonoBehaviour
{
    public int currentTargetNbr = 0;
    public int currentRoundNumber = 0;
    public int currentSessionNumber = 0;
    public int[] currentCueOrder;

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
    public GameObject[] Targets = new GameObject[6];

    //Script References
    private GameController gameController;

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


    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        //gameController.recording = true;
        GameController.SavePath = UnityEngine.Application.persistentDataPath + "/Output/" + GameController.SubjectID + "/";

        FindObjectOfType<GameController>().lokalisationTask = this;


        GameController.currentState = GameState.Task_Lokalisation_Tutorial;
    }

    void Update()
    {
        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }
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




    public void StartTask()
    {
        //ToDo: sync all variables which are updated here
        Debug.Log("Start Lokalization Task");

        currentCueOrder = OrderCues[currentSessionNumber];
        GameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
        print((Condition)currentCueOrder[currentRoundNumber]);
        maxSessionNumber = OrderCues.Count;
        maxRoundNumber = 4;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        currentTargetNbr++;
        FixationCross.SetActive(true);
        TaskReady = true;
        //ShowNextTarget();
    }








    public void SpawnTargets_LokalizationTask(int condition, int targetPosition,bool audioTest)
    {
        foreach (var t in FindObjectsOfType<Target>())
        {
            Destroy(t.gameObject);
        }
        if (GameController.currentTarget != null)
            Destroy(GameController.currentTarget);

        GameController.currentCondition = (Condition)condition;

        Targets = new GameObject[6];
        GameObject TargetContainer = GameObject.Find("GameControll/Targets");
        Targets[0] = Instantiate(TargetPrefab, TargetContainer.transform);
        Targets[0].GetComponent<Target>().defineConfiguration(-80, false); 

        Targets[1] = Instantiate(TargetPrefab, TargetContainer.transform);
        Targets[1].GetComponent<Target>().defineConfiguration(-50, false); 
        Targets[2] = Instantiate(TargetPrefab, TargetContainer.transform);
        Targets[2].GetComponent<Target>().defineConfiguration(-20, false);     
        Targets[3] = Instantiate(TargetPrefab, TargetContainer.transform);
        Targets[3].GetComponent<Target>().defineConfiguration(20, false); 
        Targets[4] = Instantiate(TargetPrefab, TargetContainer.transform);
        Targets[4].GetComponent<Target>().defineConfiguration(50, false); 
        Targets[5] = Instantiate(TargetPrefab, TargetContainer.transform);
        Targets[5].GetComponent<Target>().defineConfiguration(80, false);

        TargetContainer.transform.position = Camera.main.transform.position;


        if (targetPosition< Targets.Length)
        {
            GameController.currentTarget = Targets[targetPosition];
            Targets[targetPosition].GetComponent<Data_Targets_OT>().LT_tag = "Target";
            Targets[targetPosition].GetComponent<Target>().GiveClue(condition);
        }

        foreach (GameObject item in Targets)
        {
            if (audioTest)
            {
                item.GetComponent<SpriteRenderer>().sprite = null;
            }

            if (!item.Equals(GameController.currentTarget))
            {
               item.tag = "Untagged";
            }

        }
    }

    void ShowNextTarget()
    {
        SpawnTargets_LokalizationTask((int)GameController.currentCondition, getRandomPosition(),false);
    }


    void TargetShot(GameObject shotObject)
    {
        if (shotObject != null)
        {
            if (shotObject.tag == "Target")
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
    // TODO

    // called if the Target is Hit
    public void DefineNextTarget()
    {
        print("next");

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
            Debug.Log("Game Ends");
            GameController.currentState = GameState.End;
            FindObjectOfType<DataHandler>().writeToFile();

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
    int getRandomPosition()
    {
        return Random.Range(0, 6);
    }

}
