using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerVive : MonoBehaviour {


    Collider coll;


    private void Start()
    {
        coll = GetComponent<Collider>();
        if (coll == null)
        {
            Debug.LogWarning("No Collider attached, attach default: CAVE Proportions may be completely wrong!");
            coll = gameObject.AddComponent<BoxCollider>();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
       EventManager.CallShotEvent(collision.collider.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        print(other.gameObject);
        
        // TODO: Trigger Interactioon Event
    }
      

}
