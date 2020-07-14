
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;
using UnityEngine.PlayerLoop;

public class Target : NetworkBehaviour
{
    public Data_Targets DataContainer; // replaces TargetConfiguration.cs

    GameController gameController;
    public bool b_settingsdefined;
    public bool b_isMoving;

    public Timer deathTimer;
    AudioSource audioSource;
    [SyncVar] public bool hit;
    public Timer CueTimer;
    public int NbOfCues;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (isServer)
        {
            if (gameController.currentState == GameState.Task_Orientation_Task)
            {
                FindObjectOfType<OrientationTask>().currentTargetNbr++;
                Debug.Log("New Target: " + FindObjectOfType<OrientationTask>().currentTargetNbr);

            }
            else if (gameController.currentState == GameState.Task_Lokalisation_Task)
            {
                FindObjectOfType<LokalisationTask>().currentTargetNbr++;
                Debug.Log("New Target: " + FindObjectOfType<LokalisationTask>().currentTargetNbr);

            }
        }
        else
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        DataContainer = GetComponent<Data_Targets>();
        NbOfCues = 5;
        audioSource = GetComponent<AudioSource>();
        deathTimer = gameObject.AddComponent<Timer>();
        deathTimer.AddTimerFinishedEventListener(OutOfTime);
        deathTimer.Duration = ConfigurationUtils.TimeBetweenTargets - 0.1f;
        deathTimer.Run();
        CueTimer = gameObject.AddComponent<Timer>();
        CueTimer.Duration = 2;
        CueTimer.Run();
        CueTimer.AddTimerFinishedEventListener(RepeatCue);
    }

    void OutOfTime()
    {
        Debug.Log("Target Out of Time. Write Stats: Try to write Stats: isServer =" + isServer);

        if (isServer)
        {
            if (gameController.currentState == GameState.Task_Orientation_Task)
            {
                DataContainer.writeStats();
                Debug.Log("Target Call");
                EventManager.CallDefineNewTargetEvent();
            }
            if (gameController.currentState == GameState.Task_Lokalisation_Task)
            {
                if (gameObject.tag == "Target")
                    EventManager.CallDefineNewTargetEvent();

                DataContainer.writeStats();
            }
        }
        Debug.Log("Target Destroyed: Server= " + isServer);
        SelfDestruction();
    }

    void RepeatCue()
    {
        if (NbOfCues > 0 & hit == false)
        {
            GiveClue((int)gameController.currentCondition);
            CueTimer.Duration = 3;
            CueTimer.Run();
        }
    }

    public void GiveClue(int CueType)
    {
        if (gameObject.tag != "Target")
            return;
        NbOfCues--;
        Condition c = (Condition)CueType;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        switch (c)
        {
            case Condition.SpatialAudio:
                audioSource.spatialBlend = 1;
                audioSource.Play();
                break;
            case Condition.Tactile:
                EventManager.CallStartVibrationEvent(this.gameObject);
                break;
            case Condition.Combined:
                EventManager.CallStartVibrationEvent(this.gameObject);
                audioSource.spatialBlend = 1;
                audioSource.Play();
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (this.transform.position.y < -20)
        {
            Debug.Log("Target Removed by Falling");
            SelfDestruction();
        }
    }

    private void FixedUpdate()
    {
        //if (b_settingsdefined)
        //{
        //    MoveTarget();
        //}
    }

    public void defineConfiguration(float angle, bool moving)
    {
        transform.eulerAngles = new Vector3(0, angle, 0);
        transform.position = GameController.SpherToCart(angle);
        GetComponent<Data_Targets>().WriteStartInfo(angle, moving);
        transform.localScale = DataContainer.Size * Vector3.one;
        b_settingsdefined = true;
    }

    void MoveTarget()
    {
        int direction = DataContainer.Direction; ;
        float speed = DataContainer.Speed;
        transform.RotateAround(transform.parent.transform.position, Vector3.up, direction * speed * Time.deltaTime);
    }

    void SelfDestruction()
    {
        Debug.Log("Destroyed");
        Destroy(this.gameObject);
    }

}
