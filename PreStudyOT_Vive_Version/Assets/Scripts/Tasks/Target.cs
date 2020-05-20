
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Target : MonoBehaviour
{
    //TargetConfiguration targetConfiguration;

    public Data_Targets_OT DataContainer;
    public bool moving;

    public void Awake()
    {
        DataContainer = GetComponent<Data_Targets_OT>();
    }

    public bool b_settingsdefined;

    public Timer deathTimer;
    AudioSource audioSource;
    public bool hit;

    public void GiveClue(int CueType)
    {
        Condition c = (Condition)CueType;
        audioSource = GetComponent<AudioSource>();
        deathTimer = gameObject.AddComponent<Timer>();
        deathTimer.AddTimerFinishedEventListener(SelfDestruction);
        deathTimer.Duration = ConfigurationUtils.TimeBetweenTargets - 0.1f;
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
        //Feedback.AddTextToBottom("Cue " + c.ToString() + "At position:" + this.transform.position, true);
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
        //DataContainer = GetComponent<Data_Targets_OT>();
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
        gameObject.SetActive(false);
        //Destroy(this.gameObject);        
    }
}
