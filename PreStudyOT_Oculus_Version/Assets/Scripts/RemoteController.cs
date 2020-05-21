using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class RemoteController : NetworkBehaviour
{
	public GameObject replacementCamera;
	private GameController gameController = null;
	public NetworkManager networkManager = null;
	public GameObject target;

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
	[Command] public void CmdSpawnTarget(int condition, float angle, bool moving)
	{
		GameObject TargetContainer = GameObject.Find("GameControll/Targets");
		TargetContainer.transform.position = Camera.main.transform.position;

		GameObject instance = Instantiate(target, TargetContainer.transform);

		instance.GetComponent<Target>().defineConfiguration(angle, moving);
		instance.GetComponent<Target>().GiveClue(condition);
		NetworkServer.Spawn(instance);
		gameController.currentTarget = instance;
	}

	[Command] public void CmdDestroyCurrentTarget()
	{
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
	[Command] public void CmdSetSubjectID(string id)
	{
		gameController.SubjectID = id;
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
	[Command] public void CmdCallTargetShotEvent(GameObject target)
	{
		EventManager.CallTargetShotEvent(target);
	}
	[Command]
	public void CmdCallDefineNewTargetEvent()
	{
		EventManager.CallDefineNewTargetEvent();
	}
}
