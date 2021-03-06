﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    //Timer for Spawning
    Timer TimeBetweenTimer;

    // Target Prefab
    [SerializeField] GameObject TargetPrefab ;

    // GameObject References
    public GameObject TargetContainer;

    public static TargetSpawner instance;

    void OnEnable()
    {
        EventManager.TargetShotEvent += TargetShot;
    }
    void OnDisable()
    {
        EventManager.TargetShotEvent -= TargetShot;
    }

    void Start()
    {
        instance = FindObjectOfType<TargetSpawner>();
        TargetContainer = GameObject.FindGameObjectWithTag("TargetContainer");
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
            SpawnTarget(Random.Range(0,3),Random.Range(-90f,90));
        }
    }


    void TargetShot(GameObject shotObject)
    {
        if (shotObject != null)
        {
            if (shotObject.tag == "Target")
            {
                shotObject.tag = "Untagged";
                shotObject.GetComponent<Target>().deathTimer.Run();
                shotObject.GetComponent<Rigidbody>().useGravity = true;
                Feedback.AddTextToBottom("Target Shot", true);
                if (GameController.currentState == GameState.Task_Orientation_Task)
                {
                    EventManager.CallDefineNewTargetEvent();
                }
            }
        }
    }

   public static void SpawnTarget(int condition, float angle)
    {
        if (GameController.currentState == GameState.Task_Orientation_Tutorial|| GameController.currentState == GameState.Task_Orientation_Task)
        {
            GameController.currentTarget = instance.SpawnTarget_OrientationTask(condition, angle);
        }
        else if (GameController.currentState == GameState.Task_Lokalisation)
        {
            instance.SpawnTargets_LokalizationTask();
        }
    }

    public GameObject SpawnTarget_OrientationTask(int condition, float angle)
    {
        if (GameController.currentTarget != null)
            Destroy(GameController.currentTarget);

        GameController.currentCondition = (Condition)condition;

        instance.TargetContainer.transform.position = Camera.main.transform.position;

        GameObject NewTarget = Instantiate(instance.TargetPrefab);
        NewTarget.GetComponent<Target>().defineConfiguration(angle);
        NewTarget.transform.SetParent(instance.TargetContainer.transform, false);
        NewTarget.GetComponent<Target>().GiveClue(condition);

        return NewTarget;

    }
    GameObject SpawnTargets_LokalizationTask()
    {
        //foreach (GameObject item in GameController.Targets)
        //{
        //    Destroy(item);
        //}
        //GameController.Targets.Clear();

        //GameObject GO1 = SetTargetConfiguration(-80, 0);
        //GameObject GO2 = SetTargetConfiguration(-50, 0);
        //GameObject GO3 = SetTargetConfiguration(-20, 0);
        //GameObject GO4 = SetTargetConfiguration( 20, 0);
        //GameObject GO5 = SetTargetConfiguration( 50, 0);
        //GameObject GO6 = SetTargetConfiguration( 80, 0);

        //GameController.currentTarget = GameController.Targets[Random.Range(0, 5)];

        //foreach (GameObject item in GameController.Targets)
        //{
        //    if (!item.Equals(GameController.currentTarget))
        //        item.tag = "Untagged";
        //}
        //int condition = Random.Range(2, 5);
        //GameController.currentCondition = (Condition)condition;
        return null;// GameController.currentTarget;
    }

    GameObject SetTargetConfiguration(float anglePhi, float angleTheata)
    {
        //TargetContainer.transform.position = Camera.main.transform.position;

        //GameObject NewTarget = Instantiate(TargetPrefab);

        //NewTarget.transform.position = GameController.SpherToCart(anglePhi, angleTheata);
        //NewTarget.transform.SetParent(TargetContainer.transform, false);
        //NewTarget.GetComponent<Target>().angle = anglePhi;

        //NewTarget.transform.eulerAngles = new Vector3(0, anglePhi, 0);
        //GameController.Targets.Add(NewTarget);
        return null;// NewTarget;
    }
}
