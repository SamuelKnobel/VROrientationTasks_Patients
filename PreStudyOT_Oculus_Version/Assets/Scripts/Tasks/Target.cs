
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Target : NetworkBehaviour
{
    public Data_Targets_OT DataContainer; // replaces TargetConfiguration.cs

    GameController gameController;
    public bool b_settingsdefined;
    public bool b_isMoving;

    public Timer deathTimer;
    AudioSource audioSource;
    public bool hit;
    public Timer CueTimer;
    public int NbOfCues = 4;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        DataContainer = GetComponent<Data_Targets_OT>();
    }


    public void Start()
    {
        NbOfCues = 4;
        audioSource = GetComponent<AudioSource>();
        deathTimer = gameObject.AddComponent<Timer>();
        deathTimer.AddTimerFinishedEventListener(OutOfTime);
        deathTimer.Duration = ConfigurationUtils.TimeBetweenTargets - 0.1f;
        deathTimer.Run();
        CueTimer = gameObject.AddComponent<Timer>();
        CueTimer.Duration = 3;
        CueTimer.Run();
        CueTimer.AddTimerFinishedEventListener(RepeatCue);
    }

    void OutOfTime()
    {
        if (gameController.currentState == GameState.Task_Orientation_Task)
        {
            EventManager.CallDefineNewTargetEvent();
        }
        if (gameController.currentState == GameState.Task_Lokalisation_Task)
        {
            if (gameObject.tag == "Target")
            {
                EventManager.CallDefineNewTargetEvent();
            }
        }
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
            SelfDestruction();
        }
    }
    private void FixedUpdate()
    {
        if (b_settingsdefined)
        {
            MoveTarget();
        }
    }

    public void defineConfiguration(float angle, bool moving)
    {
        transform.eulerAngles = new Vector3(0, angle, 0);
        transform.position = GameController.SpherToCart(angle);
        GetComponent<Data_Targets_OT>().WriteStartInfo(angle, moving);
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
        // TODO MIRROR
        Destroy(this.gameObject);        
    }
}
