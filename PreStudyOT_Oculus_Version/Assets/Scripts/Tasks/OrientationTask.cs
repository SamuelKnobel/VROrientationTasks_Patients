using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;
using System.IO;

public class OrientationTask : NetworkBehaviour
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

    void Start()
    {

        gameController = GameObject.FindObjectOfType<GameController>();
        gameController.recording = true;

        gameController.SavePath = UnityEngine.Application.persistentDataPath + "/Output/" + gameController.SubjectID + "/";
        Debug.Log(gameController.SavePath);

        gameController = GameObject.FindObjectOfType<GameController>();
        localController = gameController.getLocalController();
        localController.CmdSetGameState(GameState.Task_Orientation_Tutorial);
    }

    void Update()
    {
        if (gameController== null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }
        
        // TODO! active ONLY IN TUTORIAL
        if (Input.GetKeyDown(KeyCode.T))
        {
            FixationCross.SetActive(!FixationCross.activeSelf);
            FixationCross.GetComponent<FixationCross>().TimeSeen = 0;
            FixationCross.GetComponent<FixationCross>().isSeen = false;
        }

        if (TaskReady)
        {
            gameController.currentState = GameState.Task_Orientation_Task;
            TaskReady = false;
        }
    }

    // UNCLEAR CODE, Please Explain
    public void SpawnTarget_OrientationTask(int condition, float angle, bool moving, bool audioTest)
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
            if (audioTest)
            {
                instance.GetComponent<SpriteRenderer>().sprite = null;
            }
            instance.GetComponent<Target>().defineConfiguration(angle, moving);
            instance.GetComponent<Target>().GiveClue(condition);
            NetworkServer.Spawn(instance); // ??? SPAWN here and in Remote Controller ?? are both functions neede  and used ?
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
            localController.CmdSpawnTarget(condition, angle, moving); //??? SPAWN here and in Remote Controller ?? are both functions neede and used ?
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
        //ShowNextTarget();
    }
    void ShowNextTarget()
    {
        SpawnTarget_OrientationTask((int)gameController.currentCondition, getRandomAngle(), true, false);
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
            Debug.Log("Game Ends");
            gameController.currentState = GameState.End;
            FindObjectOfType<DataHandler>().writeToFile();

        }
        else
        {
            currentCueOrder = OrderCues(currentSessionNumber);
            maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
            gameController.currentCondition= (Condition)currentCueOrder[currentRoundNumber]; 
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
