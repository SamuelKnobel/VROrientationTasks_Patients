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
    float borderLeft= 0.225f;
    float borderRight = 0.775f;  
    float borderTop= 0.775f;
    float borderBottom = 0.225f;


    // Start is called before the first frame update
    void Start()
    {
        m = new Material(Shader.Find("Diffuse"));
    }

    private void Update()
    {
        //if (isSeen&& TimeSeen <=2)
        //{
        //    ChangeColorToRed();
        //    TimeSeen += Time.deltaTime;
        //}
        //else if (TimeSeen > 2)
        //{
        //    ChangeColorToGreen();
        //}
        //else
        //{
        //    ChangeColorToRed();
        //    TimeSeen = 0;
        //}


        checkInFOV();

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
