using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using Mirror;

public class DataHandler : NetworkBehaviour
{
    // Script References
    GameController gameController;
    private SQLConnector sqlConnector;
    [SerializeField]
    public static double currentTimeStamp;

  

    //public List<Data_Targets_OT> data_Targets;

    public List<string> TargetJsons = new List<string>();
    public List<string> HardwareJsons = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        sqlConnector = new SQLConnector();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }
    }

    private void FixedUpdate()
    {
        currentTimeStamp = ConvertToTimestamp(DateTime.UtcNow);
    }

    public double ConvertToTimestamp(DateTime value)
    {
        TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        return (double)span.TotalSeconds;
    }

    public void writeHardwareToJson()
    {
        Data_Hardware[] data_Hardware = FindObjectsOfType<Data_Hardware>();
        foreach (Data_Hardware item in data_Hardware)
        {
            string NewJSON = JsonUtility.ToJson(item);
            HardwareJsons.Add(NewJSON);
            NewJSON = "";
            item.ResetAll();
        }
    }
    public void WriteTargetToJSON(Data_Targets data)
    {
        Debug.Log("target added");
        string NewJSON = JsonUtility.ToJson(data);
        TargetJsons.Add(NewJSON);
        NewJSON = "";
    }
    public string Alldata; 
    public string TargetData;
    public string HardwareData;


    public void writeToFile()
    {
        Debug.Log("Nb of Files:"+TargetJsons.Count);
        gameController.recording = false;
        string pathToDir = gameController.SavePath;
        string pathToFile = pathToDir + gameController.startTime +"_"+ gameController.SubjectID+ "_SaveData.JSON";
        if (!Directory.Exists(pathToDir))
        {
            Directory.CreateDirectory(pathToDir);
        }
        for (int i = 0; i < TargetJsons.Count - 1; i++)
        {
            TargetData += TargetJsons[i] + ",\n";
        }
        TargetData += TargetJsons[TargetJsons.Count - 1];
        writeHardwareToJson();
        for (int i = 0; i < HardwareJsons.Count - 1; i++)
        {
            HardwareData += HardwareJsons[i] + ",\n";
        }
        HardwareData += HardwareJsons[HardwareJsons.Count - 1];
        Alldata = TargetData + ',' + HardwareData;
        File.WriteAllText(pathToFile, "["+ Alldata+ "]");
        gameController.saved = true;
    }
    public void SaveToDB()
    {
        string table = (FindObjectOfType<LokalisationTask>() != null) ? "LT" : "OT";
        bool res = sqlConnector.writeToServer(table, gameController.SubjectID, gameController.startTime, TargetData);
        gameController.savingToDB = false;
        gameController.savedToDB = res;
        gameController.dbError = !res;
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
