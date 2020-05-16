using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerVive : MonoBehaviour {


    Rigidbody rb;
    Collider coll;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        if (rb == null)
        {
            Debug.LogWarning("No RigidBody attached, attach default!");
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        if (coll == null)
        {
            Debug.LogWarning("No Collider attached, attach default: CAVE Proportions may be completely wrong!");
            coll = gameObject.AddComponent<BoxCollider>();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
       EventManager.CallTargetShotEvent(collision.collider.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        print(other.gameObject);
        
        // TODO: Trigger Interactioon Event
    }
      

}
