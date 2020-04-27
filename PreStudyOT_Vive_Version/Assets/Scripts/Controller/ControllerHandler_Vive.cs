using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;


public class ControllerHandler_Vive : MonoBehaviour
{

    GameObject LeftController, RightController;
    [SerializeField]
    GameObject TargetCross_L, TargetCross_R;
    [SerializeField]
    LineRenderer LaserLeft, LaserRight;

    bool b_isConnected_left;
    bool B_isConected_Left
    {
        get { return b_isConnected_left; }
        set
        {
            if (value == b_isConnected_left)
                return;
            b_isConnected_left = value;
            if (!b_isConnected_left)
            {
                ControllerNameLeft = ControllorNotFound;
            }
            else if (b_isConnected_left)
            { 
            }
        }
    }

    bool b_isConnected_right;
    bool B_isConected_Right
    {
        get { return b_isConnected_right; }
        set
        {
            if (value == b_isConnected_right)
                return;
            b_isConnected_right = value;
            if (!b_isConnected_right)
            {
                ControllerNameRight = ControllorNotFound;
            }
            else if (b_isConnected_right)
            {

            }
        }
    }

    string ControllerNameLeft = ControllorNotFound;
    string ControllerNameRight= ControllorNotFound;
    const string ControllorNotFound = "Not Found";

    [SerializeField]
    bool b_squeeze_left, b_squeeze_right;
    [SerializeField]
    bool b_touch_left, b_touch_right;

    bool B_Squeeze_Left
    {
        get { return b_squeeze_left; }
        set
        {
            if (value == b_squeeze_left)
                return;

            b_squeeze_left = value;
            if (b_squeeze_left)
            {
                EventManager.CallTriggerLeftEvent();
            }
        }
    }
    bool B_Squeeze_Right
    {
        get { return b_squeeze_right; }
        set
        {
            if (value == b_squeeze_right)
                return;

            b_squeeze_right = value;
            if (b_squeeze_right)
            {
                EventManager.CallTriggerRightEvent();
            }
        }
    }  
    bool B_Touch_Left
    {
        get { return b_touch_left; }
        set
        {
            if (value == b_touch_left)
                return;

            b_touch_left = value;
            if (b_touch_left)
            {
                EventManager.CallTouchLeftEvent();
            }
        }
    }
    bool B_Touch_Right
    {
        get { return b_touch_right; }
        set
        {
            if (value == b_touch_right)
                return;

            b_touch_right = value;
            if (b_touch_right)
            {
                EventManager.CallTouchRightEvent();
            }
        }
    }

    Vector3 startposition_L, startposition_R;
    Vector3 direction_L, direction_R;
    Vector3 end_L, end_R;

    [SerializeField]
    float Laserdistance;

    void Start()
    {

    }
    /// <summary>
    /// Function to get left and right controller and their LineRenderer Component
    ///  TODO: Check if there is a Line Renderer
    /// </summary>
  

    // Update is called once per frame
    void Update()
    {
        B_Touch_Left = getTouchPadLeft();
        B_Touch_Right = getTouchPadRight();
        HandelController();
        B_Squeeze_Left = getSqueeze_left();
        B_Squeeze_Right = getSqueeze_right();
       
       createLaser(ConfigurationUtils.UseLaser);

       TargetCross_L.SetActive(ConfigurationUtils.UseTargetCross && B_isConected_Left);       
       TargetCross_R.SetActive(ConfigurationUtils.UseTargetCross && B_isConected_Right);       
    }
    

    /// <summary>
    /// Combines mutliple functions:
    /// - Findes the Controller GameObject
    /// - FIndes the Name of the Controllers
    /// - Checks if the Controller are connected
    /// </summary>
    void HandelController()
    {
        FindController();
        FindTargetCross();

        string[] JoyNames = Input.GetJoystickNames();
        if (ControllerNameLeft.Equals(ControllorNotFound))
        {
            ControllerNameLeft = ControllorNotFound;
            foreach (string JoystickNames in JoyNames)
            {
                if (JoystickNames.Contains("Controller") & JoystickNames.Contains("Left"))
                {
                    ControllerNameLeft = JoystickNames;
                    break;
                }
            }
        }
        if (ControllerNameRight.Equals(ControllorNotFound))
        {
            ControllerNameRight = ControllorNotFound;

            foreach (string JoystickNames in JoyNames)
            {
                if (JoystickNames.Contains("Controller") & JoystickNames.Contains("Right"))
                {
                    ControllerNameRight = JoystickNames;
                    break;
                }
            }
        }
        if (System.Array.IndexOf(JoyNames, ControllerNameLeft) != -1)
            B_isConected_Left = true;
        else
            B_isConected_Left = false;

        if (System.Array.IndexOf(JoyNames, ControllerNameRight) != -1)
            B_isConected_Right = true;
        else
            B_isConected_Right = false;
    }
    void FindController()
    {
        if (LeftController == null || RightController == null)
        {
            GameObject[] Controllers = GameObject.FindGameObjectsWithTag("Controller");
            if (Controllers.Length == 0)
            {
                Debug.LogError("No Controllers Taged");
            }
            else if ((Controllers.Length > 2))
            {
                Debug.LogError("To many Controllers Taged");
                foreach (var item in Controllers)
                {
                    Debug.Log("Contrroller:" + item.name);
                }
            }
            else
            {
                for (int i = 0; i < Controllers.Length; i++)
                {
                    if (Controllers[i].name.Contains("left"))
                    {
                        LeftController = Controllers[i];
                        LaserLeft = LeftController.GetComponentInChildren<LineRenderer>();

                    }
                    else if (Controllers[i].name.Contains("right"))
                    {
                        RightController = Controllers[i];
                        LaserRight = RightController.GetComponentInChildren<LineRenderer>();
                    }
                    else
                    {
                        Debug.Log("No Left or right Controller found, please check Name of Controller");
                    }
                }
            }
        }

    }
    void FindTargetCross()
    {
        if (TargetCross_L == null || TargetCross_R == null)
        {
            GameObject[] AllTargetCross = GameObject.FindGameObjectsWithTag("TargetCross");
            if (AllTargetCross.Length == 0)
            {
                Debug.LogError("No TargetCross Taged");
            }
            else if ((AllTargetCross.Length > 2))
            {
                Debug.LogError("To many TargetCross Taged");
                foreach (var item in AllTargetCross)
                {
                    Debug.Log("Contrroller:" + item.name);
                }
            }
            else
            {
                for (int i = 0; i < AllTargetCross.Length; i++)
                {
                    if (AllTargetCross[i].name.Contains("_R"))
                    {
                        TargetCross_R = AllTargetCross[i];
                    }
                    else if (AllTargetCross[i].name.Contains("_L"))
                    {
                        TargetCross_L = AllTargetCross[i];
                    }
                    else
                    {
                        Debug.Log("No Left or right TargetCross found, please check Name of TargetCross");
                    }
                }
            }
        }

    }

    void SideSpecificLaser(GameObject controller, GameObject targetCross, LineRenderer laser, bool squeeze)
    {
        Ray ray = new Ray(controller.transform.position, controller.transform.forward);
        RaycastHit hit;

        targetCross.transform.rotation = controller.transform.rotation;

        int layerMask = 1 << 8;  // This would cast rays only against colliders in layer 8. 
        layerMask = ~layerMask;  // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

        laser.SetPosition(0, controller.transform.position);

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            targetCross.transform.position = controller.transform.position + controller.transform.forward * Vector3.Distance(controller.transform.position, hit.point);
        }
        else
        {
            targetCross.transform.position = controller.transform.position + controller.transform.forward * 1;
        }

        if (squeeze || Input.GetKey(KeyCode.L))
        {
            if (hit.collider != null)
            {
                EventManager.CallColliderInteractionEvent(hit.collider.gameObject);

            }


            laser.SetPosition(1, targetCross.transform.position);
        }
        else
        {
            laser.SetPosition(1, controller.transform.position);
        }
    }

    public void createLaser(bool useLaser)
    {
        if (useLaser)
        {
            SideSpecificLaser(LaserLeft.gameObject, TargetCross_L, LaserLeft, B_Squeeze_Left);
            SideSpecificLaser(LaserRight.gameObject, TargetCross_R, LaserRight, B_Squeeze_Right);
        }
    }







    bool getSqueeze_left()
    {
        if (SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.LeftHand) > 0.02)
            return true;
        else
            return false;
    }  
    bool getSqueeze_right()
    {
        if (SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand) > 0.02)
                return true;
        else
            return false;

    } 
    bool getTouchPadRight()
    {
        if (SteamVR_Actions._default.Teleport.GetState(SteamVR_Input_Sources.RightHand))
                return true;
        else
            return false;

    }
    bool getTouchPadLeft()
    {
        if (SteamVR_Actions._default.Teleport.GetState(SteamVR_Input_Sources.LeftHand))
            return true;
        else
            return false;

    }
}

