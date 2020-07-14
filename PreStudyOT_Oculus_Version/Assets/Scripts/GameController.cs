using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using UnityEngine.SceneManagement;
public class GameController : NetworkBehaviour
{

	// Script References
	public List<string> parts= new List<string>();
	public RemoteController localController;
    public DataHandler dataHandler;

	// Paths
	public string SavePathBase;
	public string SavePath;

	// General Informations
	[SyncVar] public string SubjectID;
	// General Task Skript
	public bool recording = false;
	public int startTime;

	//SyncVar only works with certain types (not with ENUMs)
	[SyncVar] public int _currentCondition;
	[SyncVar] public string debugConfig;
	public Condition currentCondition
	{
		get
		{
			return (Condition)_currentCondition;
		}
		set
		{
			_currentCondition = (int)value;
		}
	}
	[SyncVar] private int _currentState;
	public GameState currentState
	{
		get
		{
			return (GameState)_currentState;
		}
		set
		{
			_currentState = (int)value;
		}
	}
	[SyncVar] public bool showFixationCross;
	[SyncVar] public GameObject currentTarget;
	[SyncVar] public bool pause;
	public GameObject pauseInfo;


	void OnEnable()
    {

        //EventManager.TriggerEvent += TriggerCalledEvent;
        EventManager.EventCue += CueCalledEvent;
    }
    void OnDisable()
    {

		//EventManager.TriggerEvent -= TriggerCalledEvent;
		EventManager.EventCue -= CueCalledEvent;
    }


    void OnAwake()
    {
        currentCondition = Condition.None;
		Time.timeScale = 1;
	}
	private Rect saveInfoBox;
	private void Start()
	{
		currentState = GameState.Initializing;
		if(SavePathBase == "")
			SavePathBase = Application.persistentDataPath;
		DontDestroyOnLoad(this.gameObject);
		getLocalController();
		OnAwake();

		dataHandler = GetComponent<DataHandler>();
		startTime = Mathf.RoundToInt((float)dataHandler.ConvertToTimestamp(DateTime.UtcNow));
		print(startTime);

		saveInfoBox = new Rect(Screen.width / 2 - 150, 10, 300, 30);
	}
	private void Update()
	{
		pauseInfo.SetActive(pause);
		if (localController == null || !localController.isLocalPlayer)
		{
			getLocalController();
		}
		if (currentState == GameState.Initializing)
		{
			currentState = GameState.MainMenu_EnterSubjectID;
		}
		checkFixationCross();
		//end task and save when pressing S
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
		{
			localController.CmdEndTaskandSave();
		}
		if (isServer)
		{
			SaveDaemon();
		}
	}

    public RemoteController getLocalController()
	{
		foreach (RemoteController controller in GameObject.FindObjectsOfType<RemoteController>())
		{
			if (controller.isLocalPlayer)
			{
				localController = controller;
				return controller;
			}
		}
		return null;
	}

	GameObject fixationCross;
	private void checkFixationCross()
	{
		if(fixationCross == null)
			fixationCross = GameObject.Find("FixationCross");
		//if(fixationCross != null)
			//fixationCross.SetActive(fixationCross.GetComponent<FixationCross>().isVisible);
	}

#region Save
	[SyncVar] public bool saving;
	[SyncVar] public bool saved;
	[SyncVar] public bool resetAfterSave = false;
	[SyncVar] public bool saveToDB;
	[SyncVar] public bool savingToDB;
	[SyncVar] public bool savedToDB;
	[SyncVar] public bool dbError;
	[SyncVar] public string dbErrorText;
	[Server]
	public void Save()
	{
		dataHandler.writeToFile();
		saving = true;
	}
	public void SaveToDB()
	{
		if (!saved && !saving)
		{
			savingToDB = false;
			savedToDB = false;
			saveToDB = true;
			Save();
		}
		else
		{
			saveToDB = false;
			savingToDB = true;
			dataHandler.SaveToDB();
		}
	}
	private void SaveDaemon()
	{
		if (saved)
		{
			saving = false;
			if(saveToDB && !savingToDB && !savedToDB)
			{
				SaveToDB();
			}
			if (resetAfterSave && (savedToDB || (!savingToDB && !saveToDB)))
			{
				saved = false;
				savedToDB = false;
				resetAfterSave = false;
				foreach (var t in FindObjectsOfType<Target>())
				{
					NetworkServer.Destroy(t.gameObject);
				}
				currentState = GameState.MainMenu_ChooseTask;
				FindObjectOfType<NetworkManager>().ServerChangeScene("MainScene");
			}
		}
	}
	#endregion

	private void OnGUI()
	{
		if (saving && !saved)
		{
			GUIStyle savingStyle = new GUIStyle("Button");
			savingStyle.normal.background = Texture2D.redTexture;
			GUILayout.BeginArea(saveInfoBox, "Saving", savingStyle);

			GUILayout.EndArea();
		}
		else if (savingToDB && !savedToDB)
		{
			GUIStyle savingStyle = new GUIStyle("Button");
			savingStyle.normal.background = Texture2D.redTexture;
			GUILayout.BeginArea(saveInfoBox, "Saving to DB", savingStyle);

			GUILayout.EndArea();
		}

		var vh = Screen.height / 100f;
		var vw = Screen.width / 100f;

		GUILayout.BeginArea(new Rect(0, 95 * vh, 100 * vw, 5 * vh));
		GUILayout.BeginHorizontal();
		GUILayout.Label("State: " + currentState.ToString());
		GUILayout.Label("SubjectID: " + SubjectID);
		string saveState = "not saved";
		if (saving) saveState = "saving to file";
		else if (saved) saveState = "saved to file";
		if (savingToDB) saveState = "saving to DB";
		else if (savedToDB) saveState = "saved to DB";
		if (dbError) saveState = "error saving to DB";
		GUILayout.Label(saveState);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}


	public void StartTutorial(GameState gameState)
	{
		switch(gameState)
		{
			case GameState.Task_Orientation:
				localController.CmdChangeScene(CompleteSceneName("OT"));
				localController.CmdSetGameState(GameState.Task_Orientation_Tutorial);
				break;
			case GameState.Task_Lokalisation:
				localController.CmdChangeScene(CompleteSceneName("LT"));
				localController.CmdSetGameState(GameState.Task_Lokalisation_Tutorial);
				break;
		}
	}

	void CueCalledEvent(float CueType)
    {
        if (currentTarget != null)
        {
            currentTarget.GetComponent<Target>().GiveClue((int)CueType);
        }
    }

	public static string CompleteSceneName(string part)
	{
		return NameFromIndex(sceneIndexFromName(part));
	}

	public static string NameFromIndex(int BuildIndex)
	{
		string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
		int slash = path.LastIndexOf('/');
		string name = path.Substring(slash + 1);
		int dot = name.LastIndexOf('.');
		return name.Substring(0, dot);
	}
	public static int sceneIndexFromName(string sceneName)
	{
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string testedScreen = NameFromIndex(i);
			//print("sceneIndexFromName: i: " + i + " sceneName = " + testedScreen);
			if (testedScreen.Contains(sceneName))
				return i;
			if (testedScreen == sceneName)
				return i;
		}
		return -1;
	}

#region Vector Conversion
	/// <summary>
	/// Converts Angles in Cartesian Coordinates
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="hor_left"></param>
	/// <param name="hor_right"></param>
	/// <param name="ver_top"></param>
	/// <param name="ver_bot"></param>
	/// <returns></returns>
	public static Vector3 SpherToCart(float angle)
    {
        float radius = ConfigurationUtils.Radius;
        float angelPhi = angle;//  y in Unity entspricht höhe, x entspricht  links/rechts, z entspricht der tiefe
        float angelTheta= 0;

        float hor_left = ConfigurationUtils.HorizontalAngleLeft;
        float hor_right = ConfigurationUtils.HorizontalAngleRight;
        float ver_top = ConfigurationUtils.VerticalAngleTop;
        float ver_bot = ConfigurationUtils.VerticalAngleBottom;

        //angelTheta = UnityEngine.Random.Range(ver_bot, ver_top) + 90;  // have to be turned by 90 degree!
        angelTheta = 0 + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
		float y_cor = 0;//radius * Mathf.Cos(angelTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }
    public static Vector3 SpherToCart( float anglePhi, float angleTheta)
    {
        float radius = ConfigurationUtils.Radius;

        angleTheta = angleTheta + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angleTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }

#endregion
}
