using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Mirror {
	public class GUIControler : NetworkBehaviour
	{
		/*
		public GameObject spherePrefab; //remove
		public GameObject replacementCamera;
		public GameObject videoPlayerPrefab;
		private GameObject videoPlayer;


		private GameController gameController = null;
		private DataHandler data;
		private string subjectID;

		private float vh, vw;
		private Rect centerRect;
		private Rect activeWindow;

		public enum GuiMode { SubjectID, TaskSelection, None, Task};
		public GuiMode currentGUI;
		// Start is called before the first frame update
		void Start()
		{
			data = NetworkManager.singleton.GetComponent<DataHandler>();
			subjectID = data.subjectID;
			vh = Screen.height / 100f;
			vw = Screen.width / 100f;
			centerRect = new Rect(30 * vw, 30 * vh, 40 * vw, 40 * vh);
			activeWindow = centerRect;
			gameController = FindObjectOfType<GameController>();
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (gameController == null)
			{
				gameController = FindObjectOfType<GameController>();
			}
			replacementCamera.SetActive(GameObject.FindGameObjectWithTag("Player") == null);
		}



		private void OnGUI()
		{
			switch(currentGUI)
			{
				case GuiMode.SubjectID:
					activeWindow = GUI.Window(0, activeWindow, guiSubjectID, "Enter Subject ID"); ;
					break;
				case GuiMode.TaskSelection:
					activeWindow = GUI.Window(1, activeWindow, guiTaskselection, "Select a Task"); ;
					break;
				case GuiMode.Task:
					activeWindow = GUI.Window(2, activeWindow, guiTaskWrapper, "Task: " + gameController.currentTaskName); ;
					break;
				default:
					break;
			}
			//guiDemo();
			guiOverview();
		}
		
		private Vector2 scrollPosition;
		void guiTaskWrapper(int WindowID)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(40*vw), GUILayout.Height(40*vh - 30));
			gameController.TaskGui();
			GUILayout.EndScrollView();
			GUI.DragWindow();

		}

		#region SubjectID
		void guiSubjectID(int windowID)
		{
			subjectID = GUILayout.TextField(subjectID);

			if (GUI.Button(new Rect(10*vw, 10*vh, 20*vw, 30), "Enter"))
			{
				CmdSetSubjectID(subjectID);
				currentGUI = GuiMode.TaskSelection;
				//ToDo: use Command
				CmdChangeGameState(GameState.MainMenu_ChooseTask);
			}

			GUI.DragWindow();
		}
		[Command]
		private void CmdChangeGameState (GameState state)
		{
			gameController.currentState = state;

		}

		[Command]
		private void CmdSetSubjectID(string id)
		{
			gameController.SubjectID = id;
		}
		#endregion

		#region Tasks
		private void guiTaskselection(int windowID)
		{
			GUILayout.BeginVertical();
			if (GUILayout.Button("Orientation"))
			{
				CmdStartTutorial(GameState.Task_Orientation);
				currentGUI = GuiMode.None;
			}
			if (GUILayout.Button("Lokalisation"))
			{
				CmdStartTutorial(GameState.Task_Lokalisation);
				currentGUI = GuiMode.None;
			}

			GUILayout.EndVertical();
			GUI.DragWindow();
		}
		[Command]
		private void CmdStartTutorial(GameState task)
		{
			gameController.StartTutorial(task);
		}
		#endregion


		private void guiDemo()
		{
			GUILayout.BeginHorizontal();

			// LAN Host
			if (GUILayout.Button("Spawn"))
			{
				CmdSpawnSphere();
			}
			if (GUILayout.Button("VideoMode"))
			{
				CmdSpawnVideoPlayer();
			}

			GUILayout.EndHorizontal();
		}


		[Command]
		void CmdSpawnSphere()
		{
			GameObject sphere = Instantiate(spherePrefab);
			NetworkServer.Spawn(sphere);
		}


		#region Video
		[Command]
		void CmdSpawnVideoPlayer()
		{
			videoPlayer = Instantiate(videoPlayerPrefab);
			NetworkServer.Spawn(videoPlayer);
		}

		[Command]
		public void CmdStartVideo(string videoURL)
		{
			VideoPlayer player = videoPlayer.GetComponent<VideoPlayer>();
			player.url = videoURL;
			//(player.prepareCompleted += CmdStartPlaying(player);
			player.Play();
		}
		#endregion


		#region Overview
		private Rect overviewRect;
		void guiOverview()
		{
			GUILayout.BeginArea(new Rect(0, 95 * vh, 100 * vw, 5 * vh));
			GUILayout.BeginHorizontal();
			GUILayout.Label("State: " + gameController != null ? gameController.currentState.ToString() : "not connected");
			if(GUILayout.Button("Subject ID: " + data.subjectID))
			{
				this.currentGUI = GuiMode.SubjectID;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		#endregion

		//ToDo: refactor TargetSpawner to call this?
		[Command]
		public void CmdSpawn(GameObject prefab)
		{
			NetworkServer.Spawn(prefab);
		}

		[Command]
		public void CmdShowFixationCross(bool show)
		{
			gameController.showFixationCross = show;
		}*/
	}
}