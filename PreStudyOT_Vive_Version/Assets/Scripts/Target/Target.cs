
using System.Collections;
using System.Collections.Generic;
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

    Rigidbody rb;

    void Start()
    {

        //GiveClue();
    }
    public void GiveClue()
    {
        audioSource = GetComponent<AudioSource>();
        deathTimer = gameObject.AddComponent<Timer>();
        deathTimer.AddTimerFinishedEventListener(SelfDestruction);
        deathTimer.Duration = ConfigurationUtils.TimeBetweenTargets - 0.1f;
        switch (GameController.currentCondition)
        {
            case Condition.NonSpatialAudio:
                audioSource.spatialBlend = 0;
                audioSource.Play();
                break;
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

    public void defineConfiguration(TargetSpace space, TargetPosition position)
    {
        switch (space)
        {
            case TargetSpace.NearSpace:
                targetConfiguration = new TargetNear(space,position);
                break;
            case TargetSpace.FarSpace:
                targetConfiguration = new TargetFar(space, position);
                break;
        }
        targetConfiguration.Initialize();
        transform.localScale = targetConfiguration.getSize()* Vector3.one;
        transform.position = GameController.SpherToCart(space, position);
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
