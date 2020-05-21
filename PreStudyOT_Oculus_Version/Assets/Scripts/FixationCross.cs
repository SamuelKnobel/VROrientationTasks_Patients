using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationCross : MonoBehaviour
{
    [SerializeField]
    MeshRenderer Part1;
    [SerializeField]
    MeshRenderer Part2;
    Material m;

    public bool isSeen;
    public float TimeSeen = 0;
    Vector3 screenPoint;
    public float borderLeft;
    public float borderRight;
    public float borderTop;
    public float borderBottom;
    // Script References
    GameController gameController;


    // Start is called before the first frame update
    void Start()
    {
        m = new Material(Shader.Find("Diffuse"));
        borderLeft = 0.275f;
        borderRight = 0.725f;
        borderTop = 0.775f;
        borderBottom = 0.225f;
}

    private void Update()
    {
        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();
        }
        if (gameController.currentState == GameState.Task_Orientation_Task || gameController.currentState == GameState.Task_Orientation_Tutorial)
        {
            checkInFOV();
            if (isSeen && TimeSeen > 2)
            {
                if (gameController.currentState == GameState.Task_Orientation_Task)
                    EventManager.CallStartSearchingEvent();

                TimeSeen = 0;
                isSeen = false;
                gameObject.SetActive(false);
            }
        }
        if (gameController.currentState == GameState.Task_Lokalisation)
        {
            checkInFOV();
            if (isSeen && TimeSeen > 2)
            {
                //if (GameController.currentState == GameState.Task_Orientation_Task)
                //    EventManager.CallStartSearchingEvent();

                TimeSeen = 0;
                isSeen = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void ChangeColorToRed()
    {
        m.color = Color.red;
        Part1.material = m;
        Part2.material = m;
    }    
    public void ChangeColorToGreen()
    {
        m.color = Color.green;
        Part1.material = m;
        Part2.material = m;
    }

    bool checkInFOV()
    {
       screenPoint = Camera.main.WorldToViewportPoint(transform.position);
       bool B_OnScreen = screenPoint.z > 0 && screenPoint.x > borderLeft && screenPoint.x <borderRight &&
            screenPoint.y > borderBottom && screenPoint.y <borderTop;
        if (B_OnScreen)
        {
            ChangeColorToGreen();
            TimeSeen += Time.deltaTime;
            isSeen = true;
        }
        else
        {
            isSeen = false;
            TimeSeen = 0;
            ChangeColorToRed();
        }

        return B_OnScreen;
    }
}
