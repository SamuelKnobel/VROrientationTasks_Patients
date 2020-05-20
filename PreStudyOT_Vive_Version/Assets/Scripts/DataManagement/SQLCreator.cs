using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;

public class SQLCreator : MonoBehaviour
{

    private void OnDisable()
    {
        if (SQLConnector.db_connection != null)
        {
            SQLConnector.Close();
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SQLConnector.ConnectToDatabase();
            CreateTables();
        }
    }

    void CreateTables()
    {
        IDbCommand dbcmd;
        dbcmd = SQLConnector.GetDbCommand();

        string q_createTable =
            "CREATE TABLE IF NOT EXISTS Subjects (id INTEGER UNIQUE PRIMARY KEY AUTOINCREMENT,subjectID STRING, timestamp FLOAT, json STRING );";
        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteReader();
        SQLConnector.Close();
    }

    public static void AddToTable(string subjectID, float timestamp, string data)
    {
        SQLConnector.ConnectToDatabase();
        IDbCommand dbcmd = SQLConnector.GetDbCommand();
        string q_createTable =
            "INSERT INTO Subjects(subjectID, timestamp, json) VALUES ('"+ subjectID+"','"+ timestamp + "','\"" + data + "\"')";
        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteReader();
        SQLConnector.Close();
    }



}
