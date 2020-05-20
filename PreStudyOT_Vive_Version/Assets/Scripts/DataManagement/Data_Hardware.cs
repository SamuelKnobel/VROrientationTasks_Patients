using Bhaptics.Tact.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Data_Hardware : MonoBehaviour
{
    [SerializeField] string identifyer = "Hardware";
    [SerializeField] public string HardwareName;
    [SerializeField] public List<Vector3> Position;
    [SerializeField] public List<Vector3> Euler_Rotation;
    [SerializeField] public List<Quaternion> Rotation;
    [SerializeField] public List<double> Timestamps;

    // Start is called before the first frame update
    void Start()
    {
        ResetAll();
        FindObjectOfType<DataHandler>().data_Hardware.Add(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameController.recording)
        {
            Position.Add(this.transform.position);
            Euler_Rotation.Add(this.transform.eulerAngles);
            Rotation.Add(this.transform.rotation);
            Timestamps.Add(DataHandler.currentTimeStamp);
            
        }
    }

    public void ResetAll()
    {
        Position.Clear();
        Position = new List<Vector3>(); 
        Euler_Rotation.Clear();
        Euler_Rotation = new List<Vector3>();
        Rotation.Clear();
        Rotation = new List<Quaternion>();   
        Timestamps.Clear();
        Timestamps = new List<double>();
    }
}
