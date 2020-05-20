using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_OT : MonoBehaviour
{
    private float vh, vw;
    private Rect centerRect;
    private Rect activeWindow;


    void Awake()
    {
        vh = Screen.height / 100f;
        vw = Screen.width / 100f;
        centerRect = new Rect(30 * vw, 30 * vh, 40 * vw, 40 * vh);
        activeWindow = centerRect;
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnGUI()
    {
        if (GameController.currentState == GameState.End)
        {
            activeWindow = GUI.Window(0, activeWindow, guiSaveAndClose, "Save and Close"); ;
        }

        ShowRecordingState();
        guiOverview();
    }



    #region Recording
    void ShowRecordingState()
    {
        GUILayout.BeginArea(new Rect(0, 90*vh, 50 * vw, 5 * vh));
        GUILayout.BeginHorizontal();
        GUILayout.Label("RecordingState: " + GameController.recording);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    #endregion
    #region SaveAndClose
    bool saving;
    bool saved;
    string ButtonText;

    private void Quit()
    {
        Debug.Log("Close Application");
        Application.Quit();

    }
    void guiSaveAndClose(int window)
    {
        if (!saving)
        {
            if (GUI.Button(new Rect(10 * vw, 10 * vh, 20 * vw, 30), "Speichern"))
            {
                ButtonText = "Quit";
                FindObjectOfType<DataHandler>().writeToFile();
                saving = true;
            }
        }
        else if (!saved)
        {
            if (GUI.Button(new Rect(10 * vw, 10 * vh, 20 * vw, 30), ButtonText))
            {
                saved = true;
                Invoke("Quit", 5);
            }
        }
        else
        {
            ButtonText = "Schliesst nach Speichern automatisch";
            GUI.Label(new Rect(10 * vw, 10 * vh, 30 * vw, 30), ButtonText);
        }
    }

    #endregion



    #region Overview
    private Rect overviewRect;
    void guiOverview()
    {
        GUILayout.BeginArea(new Rect(0, 95 * vh, 100 * vw, 5 * vh));
        GUILayout.BeginHorizontal();
        GUILayout.Label("State: " +  GameController.currentState.ToString());
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    #endregion

}
