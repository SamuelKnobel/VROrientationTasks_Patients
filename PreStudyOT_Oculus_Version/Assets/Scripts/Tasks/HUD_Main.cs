using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class HUD_Main : NetworkBehaviour
{
  
    private float vh, vw;
    private Rect centerRect;
    String subjectID;
    private Rect activeWindow;

    public enum GuiMode { SubjectID, TaskSelection, None };
    public GuiMode currentGUI;

    public GameObject AndroidWidget;

    // Script References
    GameController gameController;


    void Start()
    {
        gameController = FindObjectOfType<GameController>();
		vh = Screen.height / 100f;
        vw = Screen.width / 100f;
        centerRect = new Rect(30 * vw, 30 * vh, 40 * vw, 40 * vh);
        activeWindow = centerRect;
    }

    void Update()
    {
        if (gameController == null || !gameController.isActiveAndEnabled)
        {
            gameController = FindObjectOfType<GameController>();
        }
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
    }

    #region SubjectID


    void guiSubjectID(int windowID)
    {
        subjectID = GUILayout.TextField(subjectID);

        if (GUI.Button(new Rect(10 * vw, 10 * vh, 20 * vw, 30), "Enter"))
        {
			gameController.getLocalController().CmdSetSubjectID(subjectID);
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



}
