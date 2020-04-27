using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HUD : MonoBehaviour
{

    // InteractableUI Elements
    public Button startButtonTask1;
    public Button startButtonTask2;
    public Button generateSubjectID;
    public GameObject AndroidWidget;

    // Script References
    GameController gameController;




    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ButtonFunctions(string buttonNameString)
    {
        ButtonNames currentButton;
        bool validString = System.Enum.TryParse(buttonNameString,out currentButton);
        if (validString)
        {
            Feedback.AddTextToBottom(currentButton.ToString(), true);
            switch (currentButton)
            {
                case ButtonNames.GenerateSubjectID:
                    if (GameController.currentState == GameState.MainMenu_EnterSubjectID)
                    {
                        SubjectID_InputFieldHandler(Random.Range(0, 999).ToString());
                        generateSubjectID.gameObject.SetActive(false);
                        startButtonTask1.gameObject.SetActive(true);
                        startButtonTask2.gameObject.SetActive(true);
                    }
                    break;
                case ButtonNames.StartTask1:
                    if (GameController.currentState == GameState.MainMenu_ChooseTask)
                    {
                        startButtonTask1.gameObject.SetActive(false);
                        startButtonTask2.gameObject.SetActive(false);
                        gameController.StartTask(GameState.Task_Orientation);
                    }
                        break;
                case ButtonNames.StartTask2:
                    if (GameController.currentState == GameState.MainMenu_ChooseTask)
                    {
                        startButtonTask1.gameObject.SetActive(false);
                        startButtonTask2.gameObject.SetActive(false);
                        gameController.StartTask(GameState.Task_Lokalisation);
                    }
                    break;
                default:
                    Feedback.AddTextToBottom(currentButton + " not defined ", true);
                    break;
            }
        }
        else
            Feedback.AddTextToBottom(buttonNameString + " not defined ", true);



       

        //switch (buttonName) 
        //{
        //    //case "ResetButton":
        //    //    gameController.Restart();
        //    //    break;
        //}
    }






    public void SubjectID_InputFieldHandler(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            GameController.SubjectID = text;
            Feedback.AddTextToButton("SubjectID" + GameController.SubjectID, false);
            GameController.currentState = GameState.MainMenu_ChooseTask;
        }
    }
}
