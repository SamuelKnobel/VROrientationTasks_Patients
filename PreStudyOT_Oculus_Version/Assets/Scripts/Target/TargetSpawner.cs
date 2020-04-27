using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    //Timer for Spawning
    Timer TimeBetweenTimer;

    // Target Prefab
    public GameObject Target ;

    // GameObject References
    GameObject TargetContainer;


    // ScriptReferences
    GameController gameController;

    public static TargetSpawner instance;


    public Sprite[] TargetSprites;
    public Sprite TargetSprite;

    void OnEnable()
    {
        EventManager.ColliderInteractionEvent += TargetShotOrHit_StartCountdownTimer;
    }
    void OnDisable()
    {
        EventManager.ColliderInteractionEvent -= TargetShotOrHit_StartCountdownTimer;
    }


    // Start is called before the first frame update
    void Start()
    {
        instance = FindObjectOfType<TargetSpawner>();
        gameController = FindObjectOfType<GameController>();
        TargetContainer = GameObject.FindGameObjectWithTag("TargetContainer");

        TimeBetweenTimer = gameObject.AddComponent<Timer>();
        TimeBetweenTimer.AddTimerFinishedEventListener(SpawnTarget);
        TimeBetweenTimer.Duration = ConfigurationUtils.TimeBetweenTargets;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var t in FindObjectsOfType<Target>())
            {
                Destroy(t.gameObject);
            }
            SpawnTarget();
        }
    }
    // TO Check
    void TargetShotOrHit_StartCountdownTimer(GameObject shotObject)
    {
        if (shotObject.tag == "Target")
        {
            shotObject.GetComponent<Target>().deathTimer.Run();
            shotObject.GetComponent<Rigidbody>().useGravity = true;
            TimeBetweenTimer.Duration = ConfigurationUtils.TimeBetweenTargets;
            TimeBetweenTimer.Run();

            switch (GameController.currentState)
            {

                case GameState.Task_Orientation:
                    break;
                case GameState.Task_Lokalisation:
                    GameController.currentTarget = null;
                    break;
                default:
                    break;
            }
        }
    }

   public void SpawnTarget()
    {
        switch (GameController.currentState)
        {
            case GameState.Task_Orientation:
                 SpawnTarget_OrientationTask();
                break;

            case GameState.Task_Lokalisation:
                 SpawnTargets_LokalizationTask();
                break;

            default:
                break;
        }
    }

    public static  GameObject SpawnTarget(GameObject target, float angle)
    {
        if (GameController.currentTarget != null)
        {
            Destroy(GameController.currentTarget);
        }
        GameObject NewTarget = Instantiate(target);

        NewTarget.transform.position = GameController.SpherToCart(TargetSpace.FarSpace, angle, 0);

        return NewTarget;

    }
    GameObject SpawnTarget_OrientationTask()
    {
        int condition = Random.Range(1, 5);

        GameController.currentCondition = (Condition)condition;

        TargetContainer.transform.position = Camera.main.transform.position;
        TargetSpace newSpace;
        TargetPosition newposition; ;

        int rndSpace = Random.Range(0, 2);
        if (rndSpace == 0)
            newSpace = TargetSpace.FarSpace;
        else
            newSpace = TargetSpace.NearSpace;

        int rndLeftRight = Random.Range(0, 2);
        if (rndLeftRight ==0)
            newposition = TargetPosition.left;
        else
            newposition = TargetPosition.right;
        GameObject NewTarget = Instantiate(Target);
        NewTarget.GetComponent<Target>().defineConfiguration(newSpace, newposition);
        NewTarget.transform.SetParent(TargetContainer.transform, false);

        //NewTarget.GetComponent<SpriteRenderer>().sprite = TargetSprite;

        NewTarget.GetComponent<Target>().GiveClue((int)GameController.currentCondition);

        return NewTarget;

    }
    GameObject SpawnTargets_LokalizationTask()
    {
        foreach (GameObject item in GameController.Targets)
        {
            Destroy(item);
        }
        GameController.Targets.Clear();

        GameObject GO1 = SetTargetConfiguration(TargetSpace.FarSpace, -80, 0);
        GameObject GO2 = SetTargetConfiguration(TargetSpace.FarSpace, -50, 0);
        GameObject GO3 = SetTargetConfiguration(TargetSpace.FarSpace, -20, 0);
        GameObject GO4 = SetTargetConfiguration(TargetSpace.FarSpace, 20, 0);
        GameObject GO5 = SetTargetConfiguration(TargetSpace.FarSpace, 50, 0);
        GameObject GO6 = SetTargetConfiguration(TargetSpace.FarSpace, 80, 0);

        GameController.currentTarget = GameController.Targets[Random.Range(0, 5)];

        foreach (GameObject item in GameController.Targets)
        {
            if (!item.Equals(GameController.currentTarget))
                item.tag = "Untagged";
        }
        int condition = Random.Range(2, 5);
        GameController.currentCondition = (Condition)condition;
        return GameController.currentTarget;
    }

    GameObject SetTargetConfiguration(TargetSpace targetSpace, float anglePhi, float angleTheata)
    {
        TargetContainer.transform.position = Camera.main.transform.position;

        GameObject NewTarget = Instantiate(Target);

        NewTarget.transform.position = GameController.SpherToCart(targetSpace, anglePhi, angleTheata);
        NewTarget.transform.SetParent(TargetContainer.transform, false);
        NewTarget.GetComponent<Target>().angle = anglePhi;

        //NewTarget.GetComponent<SpriteRenderer>().sprite = TargetSprites[Random.Range(0, TargetSprites.Length)];
        NewTarget.GetComponent<SpriteRenderer>().sprite = TargetSprite;

        NewTarget.transform.eulerAngles = new Vector3(0, anglePhi, 0);
        GameController.Targets.Add(NewTarget);
        return NewTarget;
    }
}
