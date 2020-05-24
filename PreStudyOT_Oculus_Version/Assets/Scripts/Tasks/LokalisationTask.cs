using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LokalisationTask : NetworkBehaviour
{
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

    public int[] OrderCues(int i)
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


    public GameObject[] Targets = new GameObject[6];

    // Environment Elemtent References
    public GameObject FixationCross;
    [SerializeField] GameObject TargetPrefab;

    //Script References
    private GameController gameController;
    private RemoteController localController;

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


    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        //gameController.recording = true;
        gameController.SavePath = UnityEngine.Application.persistentDataPath + "/Output/" + gameController.SubjectID + "/";

        localController = gameController.getLocalController();
        localController.CmdSetGameState(GameState.Task_Lokalisation_Tutorial);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }


        if (TaskReady)
        {
            gameController.currentState = GameState.Task_Lokalisation_Task;
            TaskReady = false;
        }
    }




    [Server]
    public void StartTask()
    {
        //ToDo: sync all variables which are updated here
        Debug.Log("Start Lokalization Task");

        currentCueOrder = OrderCues(currentSessionNumber);
        gameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
        print((Condition)currentCueOrder[currentRoundNumber]);
        maxSessionNumber = NumTargetsPerRound.Count;
        maxRoundNumber = 4;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        currentTargetNbr++;
        FixationCross.SetActive(true);
        TaskReady = true;
        //ShowNextTarget();
    }








    public void SpawnTargets_LokalizationTask(int condition, int targetPosition)
    {
        foreach (var t in FindObjectsOfType<Target>())
        {
            Destroy(t.gameObject);
        }
        if (gameController.currentTarget != null)
            Destroy(gameController.currentTarget);

        gameController.currentCondition = (Condition)condition;

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
            NetworkServer.Spawn(Targets[targetPosition]);
            gameController.currentTarget = Targets[targetPosition];
            Targets[targetPosition].GetComponent<Data_Targets>().LT_tag = "Target";
            Targets[targetPosition].GetComponent<Target>().GiveClue(condition);

        }

        foreach (GameObject item in Targets)
        {
            if (!item.Equals(gameController.currentTarget))
            {
               item.tag = "Untagged";
            }
        }
    }

    void ShowNextTarget()
    {
        SpawnTargets_LokalizationTask((int)gameController.currentCondition, getRandomPosition());
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
                if (gameController.currentState == GameState.Task_Orientation_Task)
                {
                    localController.CmdCallDefineNewTargetEvent();
                }
            }
            else
            {
                if (shotObject.GetComponent<Data_Targets>() != null)
                {
                    shotObject.GetComponent<Data_Targets>().shootLog.Add(DataHandler.currentTimeStamp);
                }
            }

        }
    }
    // TODO

    // called if the Target is Hit
    [Server]
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
            gameController.currentState = GameState.End;
            FindObjectOfType<DataHandler>().writeToFile();

        }
        else
        {
            currentCueOrder = OrderCues(currentSessionNumber);
            maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
            gameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
            FixationCross.SetActive(true);
            currentTargetNbr++;
        }

    }
    int getRandomPosition()
    {
        return Random.Range(0, 6);
    }

}
