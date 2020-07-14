using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using System.Collections.Generic;
using System.Threading;

public class SQLConnector : MonoBehaviour
{

	private string _host = "159.100.247.254";
	private string _username = "ubuntu";
	private string _password = "aI7reetevcqnzru";
	private string _usernameDB = "remote";
	private string _passwordDB = "UP-0fd08hsE0";

	/* task must be "OT" or "LT" */
	public bool writeToServer(string task, string subjectID, int startTime, string json)
	{
		bool res = false;
		try
		{
			var connectionInfo = new PasswordConnectionInfo(_host, 22, _username, _password);

			using (var client = new SshClient(connectionInfo))
			{
				client.Connect();
				/*
				//dump file in log folder
				client.RunCommand($"mkdir -p log/{subjectID.Replace(" ","")}");
				client.RunCommand($"echo \"{json}\" >> log/{subjectID.Replace(" ", "")}/{startTime.Replace(" ", "")}/log.txt");
				*/
				var sql = $"INSERT INTO {task}(subjectID, timestamp, json) VALUES ('{subjectID}', '{startTime}', '')";
				var sqlCmd = client.RunCommand($"mysql -u {_usernameDB} -p{_passwordDB} -D Subjects -e \"{sql}\"");
				foreach (string chunk in Chunks(json, 20000))
				{
					sql = $"UPDATE {task} SET json = CONCAT(json, '{chunk}') WHERE timestamp = '{startTime}' AND subjectID = '{subjectID}'";
					sqlCmd = client.RunCommand($"mysql -u {_usernameDB} -p{_passwordDB} -D Subjects -e \"{sql}\"");
					
				}
				client.Disconnect();
			}
			res = true;
		}
		catch (System.Exception e)
		{
			Debug.Log(e);
			FindObjectOfType<GameController>().dbErrorText = e.ToString();
		}
		return res;
	}

    //public List<string> Chunks(string str, int maxChunkSize)
    //   {
    //	List<string> partitionedString = new List<string>();
    //       for (int i = 0; i < str.Length; i += maxChunkSize)
    //		partitionedString.Add(str.Substring(i, Math.Min(maxChunkSize, str.Length - i)));

    //	FindObjectOfType<GameController>().parts = partitionedString;

    //	return partitionedString;

    //   }
    static IEnumerable<string> Chunks(string str, int maxChunkSize)
    {
        for (int i = 0; i < str.Length; i += maxChunkSize)
        {
			yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
		}

    }
}