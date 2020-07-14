using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LokalisationTask : NetworkBehaviour
{
    [SyncVar] public int decoyNumber;
    [SyncVar] public int currentTargetNbr = 0;
    [SyncVar] public int currentRoundNumber = 0;
    [SyncVar] public int currentSessionNumber = 0;
    public int[] currentCueOrder;

    [SyncVar] public int maxTargetNbr = 0;
    [SyncVar] public int maxRoundNumber = 0;
    [SyncVar] public int maxSessionNumber = 0;
    int LastPos;

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
        EventManager.EventStartSearching += ShowNextTarget;
    }
    void OnDisable()
    {
        EventManager.EventDefineNewTarget -= DefineNextTarget;
        EventManager.EventStartSearching -= ShowNextTarget;
        EventManager.EventTargetShot -= TargetShot;
    }


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindObjectOfType<GameController>();
		gameController.recording = true;
		gameController.SavePath = gameController.SavePathBase + "/Output/" + gameController.SubjectID + "/";

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
            localController.CmdSetGameState(GameState.Task_Lokalisation_Task);
            TaskReady = false;
		}
		if (gameController.currentState == GameState.Task_Lokalisation_Tutorial && Input.GetKeyDown(KeyCode.T))
		{
			localController.CmdToggleFixationCross();
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
        //currentTargetNbr++;
        //print(currentTargetNbr);
        FixationCross.GetComponent<FixationCross>().isVisible = true;
        TaskReady = true;
    }







	private int[] spawnpositions = { -80, -50, -20, 20, 50, 80 };
    public void SpawnTargets_LokalizationTask(int condition, int targetPosition)
	{
		if (isServer)
		{
			foreach (var t in FindObjectsOfType<Target>())
			{
				NetworkServer.Destroy(t.gameObject);
            }
            // update to remove all in the array?
            if (gameController.currentTarget != null)
				NetworkServer.Destroy(gameController.currentTarget);

			gameController.currentCondition = (Condition)condition;

			GameObject TargetContainer = GameObject.Find("GameControll/Targets");
			for(int i = 0; i < 6; i++)
			{
				GameObject target = Instantiate(TargetPrefab, TargetContainer.transform);
                target.transform.SetParent(TargetContainer.transform);
				target.GetComponent<Target>().defineConfiguration(spawnpositions[i], false);
				if (targetPosition != i)
				{
					target.tag = "Decoy";
				}
				NetworkServer.Spawn(target);

                if (targetPosition == i)
                {
                    gameController.currentTarget = target;
                    target.GetComponent<Data_Targets>().LT_tag = "Target";
                    target.GetComponent<Target>().GiveClue(condition);
                }
            }
			TargetContainer.transform.position = Camera.main.transform.position;
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
			localController.CmdSpawnTargets_LokalizationTask(condition, targetPosition);
		}
	}

    void ShowNextTarget()
    {
        int randPos = getRandomPosition();
        SpawnTargets_LokalizationTask(gameController._currentCondition, randPos);
        LastPos = randPos;
    }

    public AudioClip right;
    public AudioClip wrong;
    [Server]
    void TargetShot(GameObject shotObject)
    {
        if (shotObject != null)
        {
            if (isServer)
            {
                if (shotObject.CompareTag("Target"))
                {
                    Debug.Log("LT: Target shot. Try to write Stats: isServer =" + isServer);

                    AudioSource.PlayClipAtPoint(right, shotObject.transform.position);
                    shotObject.tag = "Untagged";
                    shotObject.GetComponent<Target>().hit = true;
                    shotObject.GetComponent<Rigidbody>().useGravity = true;
                    Target[] decoys = FindObjectsOfType<Target>();
                    decoyNumber = decoys.Length;

                    foreach (Target decoy in decoys)
                    {
                        decoy.GetComponent<Target>().DataContainer.writeStats();
                        decoy.gameObject.GetComponent<Rigidbody>().useGravity = true;
				    }
                    if (gameController.currentState == GameState.Task_Lokalisation_Task)
                    {
                        localController.CmdCallDefineNewTargetEvent();
                    }
                }
                else if (shotObject.CompareTag("Decoy"))
                {
                    if(!shotObject.GetComponent<Target>().hit)
                    {
                        //shotObject.GetComponent<Target>().hit = true;
                        AudioSource.PlayClipAtPoint(wrong, shotObject.transform.position);
                    }
                    shotObject.GetComponent<Data_Targets>().shootLog.Add(DataHandler.currentTimeStamp);
                }

            }
            else
            {
                if (shotObject.tag == "Target")
                {
                    shotObject.GetComponent<Rigidbody>().useGravity = true;
                    Target[] decoys = FindObjectsOfType<Target>();
                    foreach (Target decoy in decoys)
                    {
                        decoy.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    }
                }
            }
        }
    }

    // called if the Target is Hit
    [Server]
    public void DefineNextTarget()
    {
        Debug.Log("next");
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
            localController.CmdSave();
            TaskFinished = true;
        }
        else
        {
            currentCueOrder = OrderCues(currentSessionNumber);
            maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
            gameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
            FixationCross.SetActive(true);
            FixationCross.GetComponent<FixationCross>().isVisible = true;
            //currentTargetNbr++;
        }

    }
    int getRandomPosition()
    {
        int tries=0;
        int newPost = Random.Range(0, 6);
        while (newPost == LastPos & tries < 10)
        {
            newPost = Random.Range(0, 6);
            tries++;
        }
        return newPost;
    }

}
