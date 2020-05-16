using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DataHandler : MonoBehaviour
{
    [SerializeField]
    public static double currentTimeStamp;

    public List<Data_Hardware> data_Hardware;

    //public List<Data_Targets_OT> data_Targets;

    public List<string> JSONsToSave = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameController.recording = false;
            writeHardwareToJson();
        }
    }

    private void FixedUpdate()
    {
        currentTimeStamp = ConvertToTimestamp(DateTime.UtcNow);
    }




    public static double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        return (double)span.TotalSeconds;
    }

    public void writeHardwareToJson()
    {
        foreach (Data_Hardware item in data_Hardware)
        {
            string NewJSON = JsonUtility.ToJson(item);
            JSONsToSave.Add("Hardware:"+NewJSON);
            NewJSON = "";
            item.ResetAll();
        }
        //foreach (Data_Targets_OT item in data_Targets)
        //{
        //    string NewJSON = JsonUtility.ToJson(item);
        //    JSONsToSave.Add(NewJSON);
        //    NewJSON = "";
        //    item.ResetAll();
        //}
    }
    public void WriteTargetToJSON(Data_Targets_OT data)
    {
        string NewJSON = JsonUtility.ToJson(data);
        JSONsToSave.Add("Target:" + NewJSON);
        NewJSON = "";
    }
}
[Serializable]
public enum ReasonOfDeath
{
    shot,
    hit,
    outOfTime,
    notdefined

}
