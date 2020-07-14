using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum CrossColor
{
	Default = 0,
	Green = 1,
	Red = 2
}
public class FixationCross : NetworkBehaviour
{

	[SerializeField]
	MeshRenderer Part1;
	[SerializeField]
	MeshRenderer Part2;
	Material m;

	[SyncVar] public bool isSeen;
	[SyncVar] public bool isVisible = true;
	[SyncVar(hook = nameof(SetColor))] public int _color;
	public CrossColor CurrentColor
	{
		get { return (CrossColor)_color; }
		set { _color = (int)value; }
	}
	[SyncVar] public float TimeSeen = 0;
	Vector3 screenPoint;
	public float borderLeft;
	public float borderRight;
	public float borderTop;
	public float borderBottom;
	public GameController gameController;


	// Start is called before the first frame updatSyncVare 
	void Start()
	{
		gameController = GameObject.FindObjectOfType<GameController>();
		m = new Material(Shader.Find("Diffuse"));
		borderLeft = 0.275f;
		borderRight = 0.725f;
		borderTop = 0.775f;
		borderBottom = 0.225f;
		SetColor(-1, _color);
	}

	private void Update()
	{
		Part1.GetComponent<MeshRenderer>().enabled = isVisible;
		Part2.GetComponent<MeshRenderer>().enabled = isVisible;
		
		if (isServer && isVisible)
		{
			if (gameController.currentState == GameState.Task_Orientation_Task || gameController.currentState == GameState.Task_Orientation_Tutorial ||
				gameController.currentState == GameState.Task_Lokalisation_Task || gameController.currentState == GameState.Task_Lokalisation_Tutorial)
			{
				checkInFOV();
				if (isSeen && TimeSeen > 2)
				{
					if (gameController.currentState == GameState.Task_Orientation_Task || gameController.currentState == GameState.Task_Lokalisation_Task)
						EventManager.CallStartSearchingEvent();
					TimeSeen = 0;
					isSeen = false;
					isVisible = false;
				}
			}
		}
	}

	private void SetColor(int oldVal, int newVal)
	{
		if (oldVal != newVal)
		{
			switch (newVal)
			{
				case (int)CrossColor.Red:
					m.color = Color.red;
					break;
				case (int)CrossColor.Green:
					m.color = Color.green;
					break;
			}
			Part1.material = m;
			Part2.material = m;
		}
	}

	[Server]
	bool checkInFOV()
	{
		screenPoint = Camera.main.WorldToViewportPoint(transform.position);
		bool B_OnScreen = screenPoint.z > 0 && screenPoint.x > borderLeft && screenPoint.x < borderRight &&
			 screenPoint.y > borderBottom && screenPoint.y < borderTop;
		if (B_OnScreen)
		{
			//gameController.localController.CmdUpdateFixationCross(true, CrossColor.Green, (TimeSeen + Time.deltaTime));
			isSeen = true;
			CurrentColor = CrossColor.Green;
			TimeSeen += Time.deltaTime;
		}
		else
		{
			//gameController.localController.CmdUpdateFixationCross(false, CrossColor.Red, 0);
			isSeen = false;
			CurrentColor = CrossColor.Red;
			TimeSeen = 0;
		}

		return B_OnScreen;
	}
}
