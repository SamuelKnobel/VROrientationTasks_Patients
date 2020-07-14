using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class ControllerHandler_Quest : NetworkBehaviour
{
    //[SerializeField]
    public GameObject LeftController, RightController;
    [SerializeField]    GameObject TargetCross_L, TargetCross_R;
    [SerializeField]    LineRenderer LaserLeft, LaserRight;

    public OVRInput.Button B_Squeeze_Left;
    public OVRInput.Button B_Squeeze_Right;

	[SyncVar]
	public bool leftPressed;
	[SyncVar]
	public bool rightPressed;
    private GameController gameController;
	private void Awake()
    {
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //FindController();
        //FindTargetCross();
        if (gameController == null || !gameController.isActiveAndEnabled)
        {
            gameController = FindObjectOfType<GameController>();
        }
        if (isLocalPlayer)
		{
			leftPressed = !gameController.pause && OVRInput.Get(B_Squeeze_Left);
			rightPressed = !gameController.pause && OVRInput.Get(B_Squeeze_Right);
		}
		createLaser(true);
    }
    private GameObject hitObject = null;
    void SideSpecificLaser(GameObject controller, GameObject targetCross, LineRenderer laser, bool pressed)
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
            targetCross.transform.position = controller.transform.position + controller.transform.forward * 8;
        }

        if (pressed)   
            laser.SetPosition(1, targetCross.transform.position);
        else
            laser.SetPosition(1, controller.transform.position);

        if (pressed || Input.GetKeyDown(KeyCode.L))
        {
            //print(hit.collider);
            if (hit.collider != null && hitObject != hit.collider.gameObject) {
                hitObject = hit.collider.gameObject;

                EventManager.CallTargetShotEvent(hit.collider.gameObject);
                var uiButton = hit.collider.GetComponent<Button>();
                if (uiButton != null)
                {
                    uiButton.OnSubmit(null);
                }
            }
        }
    }

    public void createLaser(bool useLaser)
    {
        if (useLaser)
        {
            SideSpecificLaser(LeftController, TargetCross_L, LaserLeft, leftPressed);
            SideSpecificLaser(RightController, TargetCross_R, LaserRight, rightPressed);
        }
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
                    if (Controllers[i].name.Contains("LeftHand"))
                    {
                        LeftController = Controllers[i];
                        LaserLeft = LeftController.GetComponentInChildren<LineRenderer>();

                    }
                    else if (Controllers[i].name.Contains("RightHand"))
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


    //private void OnCollisionEnter(Collision collision)
    //{
    //    EventManager.CallColliderInteractionEvent(collision.gameObject);
    //}

}
