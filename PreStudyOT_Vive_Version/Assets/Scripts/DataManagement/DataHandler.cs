using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using Valve.VR;

public class DataHandler : MonoBehaviour
{
    [SerializeField]
    public static double currentTimeStamp;

    public List<Data_Hardware> data_Hardware;

    public List<string> JSONsToSave = new List<string>();



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
            JSONsToSave.Add(NewJSON);
            NewJSON = "";
            item.ResetAll();
        }
    }
    public void WriteTargetToJSON(Data_Targets data)
    {
        string NewJSON = JsonUtility.ToJson(data);
        JSONsToSave.Add(NewJSON);
        NewJSON = "";
    }
    public string data;
    public void writeToFile()
    {
        GameController.recording = false;
        string pathToDir = GameController.SavePath;
        string pathToFile = pathToDir + GameController.startTime +"_"+GameController.SubjectID+ "_SaveData.JSON";
        if (!Directory.Exists(pathToDir))
        {
            Directory.CreateDirectory(pathToDir);
        }
        writeHardwareToJson();
        string data = "[";
        File.WriteAllText(pathToFile, "[");

        for (int i = 0; i < JSONsToSave.Count - 1; i++)
        {
            data = data + JSONsToSave[i] + ",\n";
            File.AppendAllText(pathToFile, JSONsToSave[i] + ",");
        }
        data = data + JSONsToSave[JSONsToSave.Count - 1] +"]";
        File.AppendAllText(pathToFile, JSONsToSave[JSONsToSave.Count - 1]);
        File.AppendAllText(pathToFile,"]");
        SQLCreator.AddToTable(GameController.SubjectID, GameController.startTime, data);

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
