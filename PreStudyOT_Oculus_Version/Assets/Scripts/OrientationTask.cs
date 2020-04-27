using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationTask : MonoBehaviour
{
    public OVRInput.Button B_X;
    public OVRInput.Button B_Y;
    public OVRInput.Button B_A;
    public OVRInput.Button B_B;
    public OVRInput.Button B_HandTrigger_R;
    public OVRInput.Button B_Menu;




    // Start is called before the first frame update
    void Start()
    {
        GameController.currentState = GameState.Task_Orientation_Tutorial;

        Feedback.AddTextToButton("Press X for first SpawnPostion", true);
        Feedback.AddTextToButton("Press Y for second SpawnPostion", false);
        Feedback.AddTextToButton("Press A for third SpawnPostion", false);
        Feedback.AddTextToButton("Press B for forth SpawnPostion", false);

        Feedback.AddTextToButton("Hold R - HandTrigger and Press X for CueType: Nothing", false);
        Feedback.AddTextToButton("Hold R - HandTrigger and Press Y for CueType: Audio", false);
        Feedback.AddTextToButton("Hold R - HandTrigger and Press A for CueType: Tactile", false);
        Feedback.AddTextToButton("Hold R - HandTrigger and Press B for CueType: Combined", false);
        Feedback.AddTextToButton("Hold MenuButton for Activating FixationCross", false);


        B_X = OVRInput.Button.Three;
        B_Y = OVRInput.Button.Four;
        B_A =OVRInput.Button.One;
        B_B = OVRInput.Button.Two;
        B_HandTrigger_R = OVRInput.Button.SecondaryHandTrigger;
        B_Menu = OVRInput.Button.Start;


    }

    // Update is called once per frame
    void Update()
    {
        if (!OVRInput.Get(B_HandTrigger_R))
        {
            if (OVRInput.GetDown(B_X)|| Input.GetKeyDown(KeyCode.Alpha1))
                SpawnObjectAtPosition(-80);
            if (OVRInput.GetDown(B_Y) || Input.GetKeyDown(KeyCode.Alpha2))
                SpawnObjectAtPosition(-40);
            if (OVRInput.GetDown(B_A) || Input.GetKeyDown(KeyCode.Alpha3))
                SpawnObjectAtPosition(40);
            if (OVRInput.GetDown(B_B) || Input.GetKeyDown(KeyCode.Alpha4))
                SpawnObjectAtPosition(80);
        }
        if (OVRInput.Get(B_HandTrigger_R) || Input.GetKey(KeyCode.G))
        {
            if (GameController.currentTarget == null)
                SpawnObjectAtPosition(-80);

            if (OVRInput.GetDown(B_X) || Input.GetKeyDown(KeyCode.Q))
                EventManager.CallCueEvent(0);

            if (OVRInput.GetDown(B_Y) || Input.GetKeyDown(KeyCode.W))
                EventManager.CallCueEvent(1);

            if (OVRInput.GetDown(B_A) || Input.GetKeyDown(KeyCode.E))
                EventManager.CallCueEvent(2);

            if (OVRInput.GetDown(B_B) || Input.GetKeyDown(KeyCode.R))
                EventManager.CallCueEvent(3);

        }
        if (OVRInput.GetDown(B_Menu) || Input.GetKeyDown(KeyCode.T))
        {
            FindObjectOfType<GameController>().FixationCross.SetActive(!FindObjectOfType<GameController>().FixationCross.activeSelf);
        }


    }



    public void SpawnObjectAtPosition(float angle)
    {
        Feedback.AddTextToBottom("SpawnObject At Angle:" + angle, true);
        GameController.currentTarget = TargetSpawner.SpawnTarget(TargetSpawner.instance.Target, angle);
    }


}
