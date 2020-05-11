using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LokalisationTask : MonoBehaviour
{

    public GameObject[] Targets = new GameObject[6];

    // Environment Elemtent References
    public GameObject FixationCross;
    public GameObject TargetContainer;
    [SerializeField] GameObject TargetPrefab;



    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GameController>().lokalisationTask = this;
        GameController.currentState = GameState.Task_Lokalisation;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnTargets_LokalizationTask(2, 0);
        }
    }

    GameObject SpawnTargets_LokalizationTask(int condition, int targetPosition)
    {
        foreach (var t in FindObjectsOfType<Target>())
        {
            Destroy(t.gameObject);
        }
        if (GameController.currentTarget != null)
            Destroy(GameController.currentTarget);

        GameController.currentCondition = (Condition)condition;

        Targets = new GameObject[6];

        Targets[0] = Instantiate(TargetPrefab);
        Targets[0].GetComponent<Target>().defineConfiguration(-80, false); 

        Targets[1] = Instantiate(TargetPrefab);
        Targets[1].GetComponent<Target>().defineConfiguration(-50, false); 
        Targets[2] = Instantiate(TargetPrefab);
        Targets[2].GetComponent<Target>().defineConfiguration(-20, false);     
        Targets[3] = Instantiate(TargetPrefab);
        Targets[3].GetComponent<Target>().defineConfiguration(20, false); 
        Targets[4] = Instantiate(TargetPrefab);
        Targets[4].GetComponent<Target>().defineConfiguration(50, false); 
        Targets[5] = Instantiate(TargetPrefab);
        Targets[5].GetComponent<Target>().defineConfiguration(80, false);

        if (targetPosition< Targets.Length)
        {
            GameController.currentTarget = Targets[targetPosition];
        }
        foreach (GameObject item in Targets)
        {
            if (!item.Equals(GameController.currentTarget))
                item.tag = "Untagged";
            item.transform.SetParent(TargetContainer.transform);
        }

        return Targets[targetPosition];
    }

    // TODO GUI For Tutorial, Gui For TaskSettings
}
