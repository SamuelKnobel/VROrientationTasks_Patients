using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Mirror;
[Serializable]
public class Data_Targets : NetworkBehaviour
{
    GameController gameController;

    [SerializeField] string identifyer = "Target";
    [SyncVar] public string LT_tag = "untagged";
    [SerializeField] GameState gameState;
    [SerializeField] Condition cueType;
    [SerializeField] double spawnTime;
    [SerializeField] Vector3 startPosition_wp;
    [SerializeField] Vector3 startPosition_lp;
    [SerializeField] float startAngle;

    [SerializeField] Vector3 endPosition_wp;
    [SerializeField] Vector3 endPosition_lp;
    [SerializeField] double deathTime;
    [SerializeField] ReasonOfDeath deathReason;
    [SerializeField] float radius;
    [SerializeField] int direction;
    [SerializeField] float speed;
    [SerializeField] float size;
    [SerializeField] public List<double> shootLog;


    private void Awake()
    {
        size = ConfigurationUtils.TargetSizeFar;
        gameController = GameObject.FindObjectOfType<GameController>();

    }
    private void Start()
    {
        gameState = gameController.currentState;
        deathReason = ReasonOfDeath.notdefined;
        cueType = gameController.currentCondition;
        spawnTime = DataHandler.currentTimeStamp;
        startPosition_wp = transform.position;
        startPosition_lp = transform.localPosition;

    }
    private void Update()
    {
        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }
    }

    private void FixedUpdate()
    {
        //if (GetComponent<Target>().hit & deathTime == 0)
        //{
        //    deathReason = ReasonOfDeath.shot;
        //    deathTime = DataHandler.currentTimeStamp;
        //    endPosition_wp = transform.position;
        //    endPosition_lp = transform.localPosition;
        //}
    }

    public void WriteStartInfo(float angle,bool moving)
    {
        startAngle = angle;
        radius = ConfigurationUtils.Radius;
        if (moving)
            speed = ConfigurationUtils.TargetSpeed;
        else
            speed = 0;
    }
    public void writeStats()
    {
        if (isServer)
        {
            if (!GetComponent<Target>().hit)
            {
                deathReason = ReasonOfDeath.outOfTime;
                deathTime = DataHandler.currentTimeStamp;
                endPosition_wp = transform.position;
                endPosition_lp = transform.localPosition;
            }
            else
            {
                deathReason = ReasonOfDeath.shot;
                deathTime = DataHandler.currentTimeStamp;
                endPosition_wp = transform.position;
                endPosition_lp = transform.localPosition;
            }
            gameController.dataHandler.WriteTargetToJSON(this);
        }
    }

    public void ResetAll()
    {
        speed = 0;
        spawnTime = 0;
        startPosition_wp = Vector3.zero;
        startPosition_lp = Vector3.zero;
        deathReason = ReasonOfDeath.notdefined;
        deathTime = 0;
        endPosition_wp = Vector3.zero;
        endPosition_lp = Vector3.zero;
    }
    public float Speed
    {
        get
        {
            return speed;
        }
    }
    public float StartAngle
    {
        get
        {
            return startAngle;
        }
    }   
    public float Size
    {
        get
        {
            return size;

        }
    }
    public int Direction
    {
        get
        {
            int direction;

            if (StartAngle < 0)
                direction = 1;
            else if (StartAngle > 0)
                direction = -1;
            else
                direction = 0;
            return direction;
        }
    }
}
