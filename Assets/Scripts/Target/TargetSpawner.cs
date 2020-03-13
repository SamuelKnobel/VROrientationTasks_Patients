using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    //Timer for Spawning
    Timer TimeBetweenTimer;

    // Target Prefab
    [SerializeField]
    GameObject Target ;

    GameController gameController;
    GameObject TargetContainer;

    public Sprite[] TargetSprites;

    void OnEnable()
    {
        EventManager.ColliderInteractionEvent += TargetShotOrHit_StartCountdownTimer;
    }

    void OnDisable()
    {
        EventManager.ColliderInteractionEvent -= TargetShotOrHit_StartCountdownTimer;
    }

    public void ChangeColliderEventListener()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        TargetContainer = GameObject.FindGameObjectWithTag("TargetContainer");

        TimeBetweenTimer = gameObject.AddComponent<Timer>();
        TimeBetweenTimer.AddTimerFinishedEventListener(EndTimer);
        TimeBetweenTimer.Duration = .5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var go in GameObject.FindGameObjectsWithTag("Target"))
            {
                Destroy(go);
            }
            SpawnTarget();
        }
    }
    void TargetShotOrHit_StartCountdownTimer(GameObject shotObject)
    {

        if (shotObject.tag == "Target")
        {
            //Destroy(shotObject);
            shotObject.GetComponent<Target>().deathTimer.Run();
            shotObject.GetComponent<Rigidbody>().useGravity = true;

            if (gameController.b_StartTask1)
            {
                TimeBetweenTimer.Duration = ConfigurationUtils.TimeBetweenTargets;
                TimeBetweenTimer.Run();
            }
            else
                gameController.currentTarget = null;

        }
    }

    void EndTimer()
    {
        SpawnTarget();

    }

    public void SpawnTarget()
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
        NewTarget.GetComponent<SpriteRenderer>().sprite = TargetSprites[Random.Range(0, TargetSprites.Length)];
        NewTarget.GetComponent<Target>().GiveClue();
        //EventManager.CallDefineSpaceEvent((int)newSpace);

    }
    public GameObject SpawnTarget(TargetSpace targetSpace, float anglePhi, float angleTheata)
    {
        TargetContainer.transform.position = Camera.main.transform.position;

        GameObject NewTarget = Instantiate(Target);

        NewTarget.transform.position = GameController.SpherToCart(targetSpace, anglePhi, angleTheata);
        NewTarget.transform.SetParent(TargetContainer.transform, false);
        NewTarget.GetComponent<Target>().angle = anglePhi;

        NewTarget.GetComponent<SpriteRenderer>().sprite = TargetSprites[Random.Range(0, TargetSprites.Length)];
        NewTarget.transform.eulerAngles = new Vector3(0, anglePhi, 0);

        return NewTarget;
    }
}
