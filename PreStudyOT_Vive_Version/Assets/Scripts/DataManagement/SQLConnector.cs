using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;

public static class SQLConnector
{

	//public static string database_name = "OT_Test.sqlite";
	//public static string db_connection_string;
	//public static IDbConnection db_connection;

	//public static void ConnectToDatabase()
	//{
	//	try
	//	{
	//		db_connection_string = "URI=file:" + Application.streamingAssetsPath + "/" + database_name;
	//		Debug.Log("db_connection_string" + db_connection_string);
	//		db_connection = new SqliteConnection(db_connection_string);
	//		db_connection.Open();
	//		Debug.Log("Connected");
	//	}
	//	catch (Exception e)
	//	{
	//		Debug.LogWarning("Could not connect To Database");
	//		throw e;
	//	}
	//}


	////helper functions
	//public static IDbCommand GetDbCommand()
	//{
	//	return db_connection.CreateCommand();
	//}

	//public static void Close()
	//{
	//	db_connection.Close();
	//}



//	void start()
//	{
//		// Create database
//		 connection = "URI=file:" + Application.streamingAssetsPath + "/" + "My_Database.sqlite";

//		// Open connection
//		IDbConnection dbcon = new SqliteConnection(connection);
//		dbcon.Open();

//		// Create table
//		IDbCommand dbcmd;
//		dbcmd = dbcon.CreateCommand();
//		string q_createTable = "CREATE TABLE IF NOT EXISTS my_table (id INTEGER PRIMARY KEY, val INTEGER )";

//		dbcmd.CommandText = q_createTable;
//		dbcmd.ExecuteReader();

//		// Insert values in table
//		//IDbCommand cmnd = dbcon.CreateCommand();
//		//cmnd.CommandText = "INSERT INTO my_table (id, val) VALUES (1, 5)";
//		//cmnd.ExecuteNonQuery();

//		// Read and print all values in table
//		IDbCommand cmnd_read = dbcon.CreateCommand();
//		IDataReader reader;
//		string query = "SELECT * FROM my_table";
//		cmnd_read.CommandText = query;
//		reader = cmnd_read.ExecuteReader();

//		while (reader.Read())
//		{
//			Debug.Log("id: " + reader[0].ToString());
//			Debug.Log("val: " + reader[1].ToString());
//		}

//		// Close connection
//		dbcon.Close();

//	}

//	// Update is called once per frame
//	void Update()
//	{

//	}
}