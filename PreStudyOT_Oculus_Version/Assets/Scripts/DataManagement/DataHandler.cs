using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    // Script References
    GameController gameController;

    [SerializeField]
    public static double currentTimeStamp;

  

    //public List<Data_Targets_OT> data_Targets;

    public List<string> JSONsToSave = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
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
            JSONsToSave.Add(NewJSON);
            NewJSON = "";
            item.ResetAll();
        }
    }
    public void WriteTargetToJSON(Data_Targets_OT data)
    {
        string NewJSON = JsonUtility.ToJson(data);
        JSONsToSave.Add(NewJSON);
        NewJSON = "";
    }
    public string data;
    public void writeToFile()
    {
        gameController.recording = false;
        string pathToDir = gameController.SavePath;
        string pathToFile = pathToDir + gameController.startTime +"_"+ gameController.SubjectID+ "_SaveData.JSON";
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
        //JSONsToSave.Clear();

        // TODO: Load it to the SQL Library Database
       // SQLCreator.AddToTable(gameController.SubjectID, gameController.startTime, data);

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
