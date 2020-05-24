﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mirror;
public class HUD_Main : NetworkBehaviour
{
  
    private float vh, vw;
    private Rect centerRect;
    string subjectID;
    private Rect activeWindow;

    public enum GuiMode { SubjectID, TaskSelection, None };
    public GuiMode currentGUI;

    // Script References
    GameController gameController;


    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
		vh = Screen.height / 100f;
        vw = Screen.width / 100f;
        centerRect = new Rect(30 * vw, 30 * vh, 40 * vw, 40 * vh);
        activeWindow = centerRect;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


    }

    private void OnGUI()
    {
        switch (currentGUI)
        {
            case GuiMode.SubjectID:
                activeWindow = GUI.Window(0, activeWindow, guiSubjectID, "Enter Subject ID"); ;
                break;
            case GuiMode.TaskSelection:
                activeWindow = GUI.Window(1, activeWindow, guiTaskselection, "Select a Task"); ;
                break;
            default:
                break;
        }
        guiOverview();
    }

    #region SubjectID
    void guiSubjectID(int windowID)
    {
        subjectID = GUILayout.TextField(subjectID);

        if (GUI.Button(new Rect(10 * vw, 10 * vh, 20 * vw, 30), "Enter"))
        {
			gameController.localController.CmdSetSubjectID(subjectID);
			gameController.localController.CmdSetGameState(GameState.MainMenu_ChooseTask);
            currentGUI = GuiMode.TaskSelection;
        }
        GUI.DragWindow();
    }

    #endregion

    #region TaskSelection
    private void guiTaskselection(int windowID)
    {

        GUILayout.BeginVertical();
        if (GUILayout.Button("Orientation"))
        {
			gameController.StartTutorial(GameState.Task_Orientation);
            currentGUI = GuiMode.None;
        }
        if (GUILayout.Button("Lokalisation"))
        {
			gameController.StartTutorial(GameState.Task_Lokalisation);
            currentGUI = GuiMode.None;
        }

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    #endregion


    #region Overview
    private Rect overviewRect;
    void guiOverview()
    {
        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();
        }
        GUILayout.BeginArea(new Rect(0, 95 * vh, 100 * vw, 5 * vh));
        GUILayout.BeginHorizontal();
        GUILayout.Label("State: " + gameController != null ? gameController.currentState.ToString() : "not connected");
        if (GUILayout.Button("Subject ID: " + gameController.SubjectID))
        {
            this.currentGUI = GuiMode.SubjectID;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    #endregion

}
