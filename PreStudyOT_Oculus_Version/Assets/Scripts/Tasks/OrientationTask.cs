using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;
using UnityEngine.SocialPlatforms;
using System.Runtime.InteropServices.WindowsRuntime;

public class OrientationTask : NetworkBehaviour
{
    [SyncVar] public int currentTargetNbr = 0;
	[SyncVar] public int currentRoundNumber = 0;
	[SyncVar] public int currentSessionNumber = 0;
	public int[] currentCueOrder;

	[SyncVar] public int maxTargetNbr = 0;
	[SyncVar] public int maxRoundNumber = 0;
	[SyncVar] public int maxSessionNumber = 0;
	public float lastPosition;

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
    public GameObject TargetContainer;
    public GameObject TargetPrefab;
	
    //Script References
	private GameController gameController;
	private RemoteController localController;

	
	private void OnEnable()
	{
		EventManager.EventTargetShot += TargetShot;
		EventManager.EventDefineNewTarget += DefineNextTarget;
		EventManager.EventStartSearching += ShowNextTarget;
	}
	private void OnDisable()
	{
		EventManager.EventDefineNewTarget -= DefineNextTarget;
		EventManager.EventStartSearching -= ShowNextTarget;
		EventManager.EventTargetShot -= TargetShot;
	}

	void Start()
	{
		gameController = GameObject.FindObjectOfType<GameController>();
        gameController.recording = true;
		
        gameController.SavePath = gameController.SavePathBase + "/Output/" + gameController.SubjectID + "/";
        Debug.Log(gameController.SavePath);

		localController = gameController.getLocalController();
		localController.CmdSetGameState(GameState.Task_Orientation_Tutorial);
	}

	void Update()
	{
        if (gameController== null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }

		if (TaskReady)
		{
			localController.CmdSetGameState(GameState.Task_Orientation_Task);
            TaskReady = false;
		}
		if (gameController.currentState == GameState.Task_Orientation_Tutorial && Input.GetKeyDown(KeyCode.T))
		{
			localController.CmdToggleFixationCross();
		}

	}

	/*
	 * The Spawn Code is available here and in Remotecontroller
	 * to spawn from the Client, a Command has to be used to execute the Code on the Server
	 * If this function is called on the Server (e.g. triggered by the event system), the code is run here,
	 * if the function is called on the Client (e.g. Tutorial) it uses the Remotecontroller which has the Authority to invoke Commands
	 */
    public void SpawnTarget_OrientationTask(int condition, float angle, bool moving, bool audioTest)
    {

		if (isServer) {

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
                instance.GetComponent<SpriteRenderer>().sprite = null; //might have to use a syncvar
            }
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
			localController.CmdSpawnTarget(condition, angle, moving, audioTest);
		}
    }


	[Server]
    public void StartTask()
    {
		//ToDo: sync all variables which are updated here
        Debug.Log("StartTask");
		currentCueOrder = OrderCues(currentSessionNumber);
		gameController.currentCondition = (Condition)currentCueOrder[currentRoundNumber];
        maxSessionNumber = NumTargetsPerRound.Count;
        maxRoundNumber = 4;
        maxTargetNbr = NumTargetsPerRound[currentSessionNumber];
        //currentTargetNbr++;
        FixationCross.GetComponent<FixationCross>().isVisible = true;
		TaskReady = true;
	}

    void ShowNextTarget()
    {
		float NewPos = getRandomAngle();
		SpawnTarget_OrientationTask(gameController._currentCondition, NewPos, true, false);
		lastPosition = NewPos;
	}


    void TargetShot(GameObject shotObject)
    {
        if (shotObject != null)
        {
            if (shotObject.tag == "Target")
            {
				Debug.Log("OT: Target shot. Try to write Stats: isServer =" + isServer);
                if (isServer)
                {
					shotObject.GetComponent<Target>().hit = true;
					shotObject.tag = "Untagged";
					shotObject.GetComponent<Target>().DataContainer.writeStats();
					if (gameController.currentState == GameState.Task_Orientation_Task)
					{
						localController.CmdCallDefineNewTargetEvent();
					}
				}
				shotObject.GetComponent<Rigidbody>().useGravity = true;
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
        }
	}

    float getRandomAngle()
    {
		int tries = 0;
		//return 0;
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

        while (spawnAngle == lastPosition & tries < 10)
        {
			tries++;
			pos = Random.Range(0, 4);
			if (pos == 0)
				spawnAngle = -70f;
			else if (pos == 1)
				spawnAngle = -30f;
			else if (pos == 2)
				spawnAngle = 30f;
			else if (pos == 3)
				spawnAngle = 70f;
		}
        return spawnAngle;		
    }

}
