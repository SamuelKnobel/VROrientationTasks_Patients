using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState 
{
     Initializing,     // MainMenu State should not be entered until a Connection to the server has be established!

    MainMenu_EnterSubjectID,
    MainMenu_ChooseTask,
    Task_Orientation,
    Task_Orientation_Tutorial,
    Task_Orientation_Task,
    Task_Lokalisation,
    End
}
