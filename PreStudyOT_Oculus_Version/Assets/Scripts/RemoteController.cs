using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RemoteController : NetworkBehaviour
{
	public GameObject replacementCamera;
	private GameController gameController = null;
	public NetworkManager networkManager = null;
	public GameObject targetPrefab;
	private FixationCross fixationCross;

	void Awake()
    {
		gameController = FindObjectOfType<GameController>();
		networkManager = FindObjectOfType<NetworkManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if(gameController == null || !gameController.isActiveAndEnabled)
		{
			gameController = FindObjectOfType<GameController>();
		}
		if (replacementCamera != null)
		{
			replacementCamera.SetActive(gameObject.tag != "Player" && GameObject.FindGameObjectWithTag("Player") == null);
		}

	}


	[Command] public void CmdChangeScene(string sceneName)
	{
		networkManager.ServerChangeScene(sceneName);
	}
	[Command] public void CmdDestroy(GameObject target)
	{
		NetworkServer.Destroy(target);
	}
	[Command] public void CmdSpawnTarget(int condition, float angle, bool moving, bool audioTest)
	{
		GameObject TargetContainer = GameObject.Find("GameControll/Targets");
		TargetContainer.transform.position = Camera.main.transform.position;

		GameObject instance = Instantiate(targetPrefab, TargetContainer.transform);
		if (audioTest)
            {
                instance.GetComponent<SpriteRenderer>().sprite = null;
            }
		instance.GetComponent<Target>().defineConfiguration(angle, moving);
		instance.GetComponent<Target>().GiveClue(condition);
		NetworkServer.Spawn(instance);
		gameController.currentTarget = instance;
	}
	
	private int[] spawnpositions = { -80, -50, -20, 20, 50, 80 };
	[Command] public void CmdSpawnTargets_LokalizationTask(int condition, int targetPosition)
	{
		GameObject TargetContainer = GameObject.Find("GameControll/Targets");
		for (int i = 0; i < 6; i++)
		{
			GameObject target = Instantiate(targetPrefab, TargetContainer.transform);
			target.transform.SetParent(TargetContainer.transform);
			target.GetComponent<Target>().defineConfiguration(spawnpositions[i], false);
			if (targetPosition != i)
			{
				target.tag = "Decoy";
			}
			if (targetPosition == i)
			{
				gameController.currentTarget = target;
				target.GetComponent<Data_Targets>().LT_tag = "Target";
				target.GetComponent<Target>().GiveClue(condition);
			}
			NetworkServer.Spawn(target);
		}
		TargetContainer.transform.position = Camera.main.transform.position;
	}

	[Command] public void CmdDestroyCurrentTarget()
	{
		FindObjectOfType<OrientationTask>().currentTargetNbr = 0;
		NetworkServer.Destroy(gameController.currentTarget);
		gameController.currentTarget = null;
	}

	[Command] public void CmdSetGameState(GameState state)
	{
		gameController.currentState = state;
	}
	[Command] public void CmdSetCondition(Condition condition)
	{
		gameController.currentCondition = condition;
	}
	[Command] public void CmdSetSubjectID(string subjectID)
	{
		FindObjectOfType<GameController>().SubjectID = subjectID;
	}
	[Command] public void CmdSyncTaskSetupOT(int numOfSessions, int[] objectsPerRound, int[] cueOrder1, int[] cueOrder2, int[] cueOrder3, int[] cueOrder4)
	{
		OrientationTask ot = GameObject.FindObjectOfType<OrientationTask>();
		ot.NumTargetsPerRound.Clear();
		ot.OrderCues1.Clear();
		ot.OrderCues2.Clear();
		ot.OrderCues3.Clear();
		ot.OrderCues4.Clear();
		for (int i = 0; i < numOfSessions; i++)
		{
			ot.NumTargetsPerRound.Add(objectsPerRound[i]);
		}
		for (int i = 0; i < 4; i++)
		{
			if (numOfSessions > 0) ot.OrderCues1.Add(cueOrder1[i]);
			if (numOfSessions > 1) ot.OrderCues2.Add(cueOrder2[i]);
			if (numOfSessions > 2) ot.OrderCues3.Add(cueOrder3[i]);
			if (numOfSessions > 3) ot.OrderCues4.Add(cueOrder4[i]);
		}
		ot.maxSessionNumber = numOfSessions;
	}
	[Command] public void CmdSetOTReady(bool ready)
	{
		GameObject.FindObjectOfType<OrientationTask>().TaskReady = ready;
	}
	[Command] public void CmdStartTaskOT()
	{
		GameObject.FindObjectOfType<OrientationTask>().StartTask();
	}
	//[Command] public void CmdCallTargetShotEvent(GameObject target)
	//{
	//	EventManager.CallTargetShotEvent(target);
	//}
	[Command] public void CmdCallDefineNewTargetEvent()
	{
		Debug.Log("RemoteControler Call from Server:" +isServer );
		EventManager.CallDefineNewTargetEvent();
	}
	[Command] public void CmdUpdateFixationCross(bool isSeen, CrossColor color, float timeSeen)
	{
		if (fixationCross == null)
		{
			fixationCross = FindObjectOfType<FixationCross>();
		}
		if (fixationCross != null)
		{
			fixationCross.isSeen = isSeen;
			fixationCross.CurrentColor = color;
			fixationCross.TimeSeen = timeSeen;
		}
	}
	[Command]
	public void CmdEnableFixationCross(bool enabled)
	{
		if (fixationCross == null)
		{
			fixationCross = FindObjectOfType<FixationCross>();
		}
		if (fixationCross != null)
		{
			fixationCross.isVisible = enabled;
		}
	}
	[Command]
	public void CmdToggleFixationCross()
	{
		fixationCross = FindObjectOfType<FixationCross>();
		fixationCross.TimeSeen = 0;
		fixationCross.isSeen = false;
		fixationCross.isVisible = !fixationCross.isVisible;
	}

	[Command] public void CmdCallStartSearching()
	{
		EventManager.CallStartSearchingEvent();
	}

	[Command]
	public void CmdStartTaskLT()
	{
		GameObject.FindObjectOfType<LokalisationTask>().StartTask();
	}
	
	[Command]
	public void CmdSyncTaskSetupLT(int numOfSessions, int[] objectsPerRound, int[] cueOrder1, int[] cueOrder2, int[] cueOrder3, int[] cueOrder4)
	{
		LokalisationTask lt = GameObject.FindObjectOfType<LokalisationTask>();
		lt.NumTargetsPerRound.Clear();
		lt.OrderCues1.Clear();
		lt.OrderCues2.Clear();
		lt.OrderCues3.Clear();
		lt.OrderCues4.Clear();
		for (int i = 0; i < numOfSessions; i++)
		{
			lt.NumTargetsPerRound.Add(objectsPerRound[i]);
		}
		for (int i = 0; i < 4; i++)
		{
			if (numOfSessions > 0) lt.OrderCues1.Add(cueOrder1[i]);
			if (numOfSessions > 1) lt.OrderCues2.Add(cueOrder2[i]);
			if (numOfSessions > 2) lt.OrderCues3.Add(cueOrder3[i]);
			if (numOfSessions > 3) lt.OrderCues4.Add(cueOrder4[i]);
		}
		lt.maxSessionNumber = numOfSessions;
	}

	[Command] public void CmdEndTaskandSave()
	{
		gameController.pause = false;
		gameController.resetAfterSave = false;
		gameController.SaveToDB();
	}
	[Command]
	public void CmdSave()
	{
		gameController.Save();
	}
	[Command] public void CmdSaveToDB()
	{
		gameController.resetAfterSave = false;
		gameController.SaveToDB();
	}

	[Command] public void CmdTogglePause()
	{
		gameController.pause = !gameController.pause;
	}
}
