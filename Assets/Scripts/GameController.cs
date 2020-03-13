using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Script References
    [SerializeField]
    GameObject StartCube;
    TargetSpawner spawner;

    // UI References : TODO-->make the own UI script
    public Button RestartButton;

    // General Task Skript
    public static Condition currentCondition;

    // Task 1
    public bool b_StartTask1;
    Timer CountdownTimerTask1;

    // Task 2
    public List<GameObject> Targets = new List<GameObject>();
    public GameObject currentTarget;
    public bool b_StartTask2;
    Timer CountdownTimerTask2;


    void OnEnable()
    {
        EventManager.ColliderInteractionEvent += StartCountdownTimerTask1;
        EventManager.ColliderInteractionEvent += StartTimerTask2; ;
        EventManager.TouchLeftEvent += GiveClue;
        EventManager.TouchRightEvent += GiveClue;
    }
    void OnDisable()
    {
        EventManager.ColliderInteractionEvent -= StartCountdownTimerTask1;
        EventManager.ColliderInteractionEvent -= StartTimerTask2; ;
        EventManager.TouchLeftEvent -= GiveClue;
        EventManager.TouchRightEvent -= GiveClue;
    }

    private void Awake()
    {
        // TODO: make  things dependent from Type of HMD -->Use ENUM
        DefineHMD();

    }
    void Start()
    {
        if (FindObjectOfType<GameController>() == null)
        {
                DontDestroyOnLoad(this);
        }
        else
        {
            print("multiple scripts found("+ FindObjectsOfType<GameController>().Length + "), destroy the additional");
            Destroy(this);
        }
        OnStart();
    }
    void OnStart()
    {
        spawner = FindObjectOfType<TargetSpawner>();
        currentCondition = Condition.None;
        GenerateStartButtons();
        CountdownTimerTask2 = gameObject.AddComponent<Timer>();
        CountdownTimerTask2.AddTimerFinishedEventListener(StartTask2);
    }

    void GenerateStartButtons()
    {
        if (GameObject.Find("start1") == null)
        {
            GameObject start1 = Instantiate(StartCube);
            start1.name = "start1";
        }
        if (GameObject.Find("start2") == null)
        {
            GameObject start2 = Instantiate(StartCube);
            start2.transform.position += new Vector3(1, 0, 0);
            start2.name = "start2";

            Material m = new Material(Shader.Find("Diffuse"));
            m.color = Color.green;
            start2.GetComponent<MeshRenderer>().material = m;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void StartTimerTask2(GameObject go)
    {
        if (go.tag == "Target")
        {
            CountdownTimerTask2.Duration = 2;
            CountdownTimerTask2.Run();
        }
    }
    void StartCountdownTimerTask1(GameObject go)
    {
        if (go.name.Contains("start1")) 
        {
            if (!b_StartTask1)
            {
                CountdownTimerTask1 = gameObject.AddComponent<Timer>();
                CountdownTimerTask1.AddTimerFinishedEventListener(StartTask1);
                CountdownTimerTask1.Duration = 2;
                CountdownTimerTask1.Run();
                Destroy(go);
                Destroy(GameObject.Find("start2"));
                EventManager.ColliderInteractionEvent -= StartTimerTask2; 
            }
        }     
        if (go.name.Contains("start2")) 
        {
            if (!b_StartTask2)
            {
                print("start Task2"); 
                b_StartTask2 = true;
                StartTask2();
                Destroy(go);
                Destroy(GameObject.Find("start1"));
                EventManager.ColliderInteractionEvent -= StartCountdownTimerTask1;
               
            }
        }
    }


    public void Restart()
    {
        currentTarget = null;
        currentCondition = Condition.None;
        EventManager.ColliderInteractionEvent -= StartCountdownTimerTask1;
        EventManager.ColliderInteractionEvent -= StartTimerTask2; 
        EventManager.ColliderInteractionEvent += StartCountdownTimerTask1;
        EventManager.ColliderInteractionEvent += StartTimerTask2;
        b_StartTask1 = false;
        b_StartTask2 = false;
        GameObject[] tars = GameObject.FindGameObjectsWithTag("Target");
        for (int i = tars.Length-1; i >=0 ; i--)
        {
            Destroy(tars[i].gameObject);
        }
        foreach (GameObject item in Targets)
        {
            Destroy(item);
        }
        Targets.Clear();
        GenerateStartButtons();
        GenerateStartButtons();


    }
    void StartTask1()
    {
      b_StartTask1 = true;
        Destroy(CountdownTimerTask1);
        spawner.SpawnTarget();

    }

    void StartTask2()
    {
        if (b_StartTask2)
        {
            foreach (GameObject item in Targets)
            {
                //item    .GetComponent<Rigidbody>().useGravity = true;
                Destroy(item);
            }
            Targets.Clear();

            GameObject GO1 = spawner.SpawnTarget(TargetSpace.FarSpace, -80, 0);
            GameObject GO2 = spawner.SpawnTarget(TargetSpace.FarSpace, -50, 0);
            GameObject GO3 = spawner.SpawnTarget(TargetSpace.FarSpace, -20, 0);
            GameObject GO4 = spawner.SpawnTarget(TargetSpace.FarSpace, 20, 0);
            GameObject GO5 = spawner.SpawnTarget(TargetSpace.FarSpace, 50, 0);
            GameObject GO6 = spawner.SpawnTarget(TargetSpace.FarSpace, 80, 0);
            Targets.Add(GO1);
            Targets.Add(GO2);
            Targets.Add(GO3);
            Targets.Add(GO4);
            Targets.Add(GO5);
            Targets.Add(GO6);
            currentTarget = Targets[Random.Range(0, 5)];

            foreach (GameObject item in Targets)
            {
                if (!item.Equals(currentTarget))
                    item.tag = "Untagged";
            }
            int condition = Random.Range(2, 5);
            GameController.currentCondition = (Condition)condition;
        }
    }

    public void GiveClue()
    {
        if (currentTarget != null&& b_StartTask2)
        {
            currentTarget.GetComponent<Target>().GiveClue();
        }
    }



    /// <summary>
    /// Converts Angles in Cartesian Coordinates
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="hor_left"></param>
    /// <param name="hor_right"></param>
    /// <param name="ver_top"></param>
    /// <param name="ver_bot"></param>
    /// <returns></returns>
    public static Vector3 SpherToCart(TargetSpace targetSpace, TargetPosition targetPosition)
    {
        float radius = 0;
        float angelPhi = 0;
        float angelTheta= 0;

        float hor_left = ConfigurationUtils.HorizontalAngleLeft;
        float hor_right = ConfigurationUtils.HorizontalAngleRight;
        float ver_top = ConfigurationUtils.VerticalAngleTop;
        float ver_bot = ConfigurationUtils.VerticalAngleBottom;
        switch (targetSpace)
        {
            case TargetSpace.NearSpace:
                //radius = ConfigurationUtils.RadiusNearspace;
                radius =.55f;
                Debug.LogWarning("Radius Near Space Hardcoded");
                break;
            case TargetSpace.FarSpace:
                radius = ConfigurationUtils.RadiusFarspace;
                break;
        }
        switch (targetPosition)
        {
            case TargetPosition.left:
                angelPhi = hor_left;//  y in Unity entspricht höhe, x entspricht  links/rechts, z entspricht der tiefe
                break;
            case TargetPosition.right:
                angelPhi = hor_right;
                break;
        }

        angelTheta = UnityEngine.Random.Range(ver_bot, ver_top) + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angelTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(angelPhi / 180 * Mathf.PI) * Mathf.Sin(angelTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }
    public static Vector3 SpherToCart(TargetSpace targetSpace, float anglePhi, float angleTheta)
    {
        float radius = 0;


        switch (targetSpace)
        {
            case TargetSpace.NearSpace:
                radius = ConfigurationUtils.RadiusNearspace;
                break;
            case TargetSpace.FarSpace:
                radius = ConfigurationUtils.RadiusFarspace;
                break;
        }

        angleTheta = angleTheta + 90;  // have to be turned by 90 degree!

        float x_cor = radius * Mathf.Sin(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        float y_cor = radius * Mathf.Cos(angleTheta / 180 * Mathf.PI);
        float z_cor = radius * Mathf.Cos(anglePhi / 180 * Mathf.PI) * Mathf.Sin(angleTheta / 180 * Mathf.PI);
        return new Vector3(x_cor, y_cor, z_cor);
    }

    //public static Vector3 CartToSpher(Vector3 position)
    //{

    //    float anglePhi=0;
    //    float angleTheta = 0;
    //    float radius = 0;




    //    return new Vector3(anglePhi, angleTheta, radius);
    //}

    void DefineHMD()
    {
        string hardware;

        //string model = UnityEngine.VR.VRDevice.model != null ? UnityEngine.VR.VRDevice.model : "";
        string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";
        if (model.IndexOf("Vive") >= 0)
        {
            hardware = "htc_vive";
        }
        else
        {
            hardware = "Others";
        }

        //print(UnityEngine.XR.XRDevice.model);

    }

}
