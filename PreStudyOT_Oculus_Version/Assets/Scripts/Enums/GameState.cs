using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState 
{
     Initializing,     // MainMenu State should not be entered until a Connection to the server has be established!

    MainMenu_EnterSubjectID = 1,
    MainMenu_ChooseTask = 2,
    Task_Orientation = 3,
    Task_Orientation_Tutorial = 4,
    Task_Orientation_Task = 5,
    Task_Lokalisation = 6,
    Task_Lokalisation_Tutorial = 7,
    Task_Lokalisation_Task = 8,
    End = 10
}
