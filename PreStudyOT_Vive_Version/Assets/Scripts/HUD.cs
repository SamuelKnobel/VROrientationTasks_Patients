using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HUD : MonoBehaviour
{
    // UI elements
    Button ResetButton;
    Button StartTask1,StartTask2;
    InputField inputField_SubjectID;
    List<Button> HUD_Buttons= new List<Button>();


    // Script References
    GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        AllocateUIElements();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }  
    }

    public void ButtonFunctions()
    {
        string currentname = EventSystem.current.currentSelectedGameObject.name;


        switch (currentname)
        {
            case "ResetButton":
                gameController.Restart();
                break;

            case "Start_Task1":
                gameController.startOrientationTask();
                    break;

            case "Start_Task2":
                gameController.startLocalizationTask();
                break;
            default:
                Debug.LogWarning(currentname + " not defined ");
                break;
        }
    }

    void AllocateUIElements()
    {
        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            if (button.name == "ResetButton")
            {
                ResetButton = button;
                HUD_Buttons.Add(button);

            }
            if (button.name == "Start_Task1")
            {
                StartTask1 = button;
                HUD_Buttons.Add(button);
            }
            if (button.name == "Start_Task2")
            {
                StartTask2 = button;
                HUD_Buttons.Add(button);
            }
        }

        InputField[] inputFields = FindObjectsOfType<InputField>();
        foreach (InputField field in inputFields)
        {
            if (field.name == "InputField_SubjectID")
            {
                inputField_SubjectID = field;
            }
        }   
    }


    public void SubjectID_InputFieldHandler(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            inputField_SubjectID.text = text;
            GameController.SubjectID = text;
        }
    }
}
