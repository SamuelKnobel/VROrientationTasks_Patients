
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    public TargetConfiguration targetConfiguration;

    public float angle;

    public bool b_settingsdefined;
    public bool b_isMoving;

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
        Feedback.AddTextToBottom("Cue " + c.ToString() + "At position:" + this.transform.position, true);
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

    public void defineConfiguration(float angle)
    {
        TargetPosition position;
        if (angle < 0)
            position = TargetPosition.left;
        else
            position = TargetPosition.right;

        transform.eulerAngles = new Vector3(0, angle, 0);
        targetConfiguration = new TargetConfiguration(position);
        targetConfiguration.Initialize();
        transform.localScale = targetConfiguration.getSize()* Vector3.one;
        transform.position = GameController.SpherToCart(angle);
        b_settingsdefined = true;
    }


    void MoveTarget()
    {
        int direction = targetConfiguration.getDirection();
        float speed = targetConfiguration.getSpeed();
        transform.RotateAround(transform.parent.transform.position, Vector3.up, direction*speed * Time.deltaTime);
        b_isMoving = true;
    }

    void SelfDestruction()
    {
        Destroy(this.gameObject);        
    }
}
