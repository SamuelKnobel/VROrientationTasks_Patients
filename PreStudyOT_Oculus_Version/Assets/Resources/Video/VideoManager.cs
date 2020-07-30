using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;

namespace Mirror
{
	[System.Serializable]
	public struct VideoURL
	{
		public string name;
		public string url;
	}

	[System.Serializable]
	public class SyncedVideoList : SyncList<VideoURL> { }

	public class VideoManager : NetworkBehaviour
	{
		public readonly SyncedVideoList videoList = new SyncedVideoList();
		[SyncVar]
		public double length = 0;
		[SyncVar]
		public double current = 0;
		[SyncVar]
		public string selectedURL = "";

		public string path = "Videos";
		public VideoPlayer videoPlayer;
		public GameObject text;
		public Material skyboxMaterialDefault;
		public Material skyboxMaterialVideo;
		private Vector2 scrollPosition;
		public GUIControler localPlayer;
		// Start is called before the first frame update
		void Start()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
				RenderSettings.skybox = skyboxMaterialVideo;
				DirectoryInfo dir = new DirectoryInfo("/mnt/sdcard/" + path);
				
				//DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
				//DirectoryInfo dir = new DirectoryInfo("../Videos");
				FileInfo[] files = dir.GetFiles("*.*");
				foreach (FileInfo file in files)
				{
					VideoURL video = new VideoURL
					{
						name = file.Name.Remove(file.Name.Length - file.Extension.Length),
						url = file.FullName
					};
					//text.GetComponent<TextMeshPro>().text = Application.streamingAssetsPath;
					//CmdAddVideo(video);
					videoList.Add(video);
				}
				//videoPlayer.url = res;
				//videoPlayer.Play();
				RenderSettings.skybox = skyboxMaterialVideo;
#endif
		}

		// Update is called once per frame
		private void Update()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
				current = videoPlayer.time;
				length = videoPlayer.length;
				selectedURL = videoPlayer.url;
			#endif
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(10, 50, 200, 280));
			GUILayout.BeginVertical("box");
			GUILayout.Label("Videos:");
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(190), GUILayout.Height(200));
			GUILayout.BeginVertical();
			// LAN Host

			foreach (VideoURL video in videoList)
			{
				if (video.url == selectedURL)
				{
					GUIStyle active = new GUIStyle();
					active.normal.textColor = Color.green;
					GUILayout.Button(video.name, active);
				}
				else
				{
					if (GUILayout.Button(video.name))
					{

						GUIControler localPlayer = GameObject.Find("GUIControler(Clone)").GetComponent<GUIControler>();
						//localPlayer.CmdStartVideo(video.url);
					}
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.Label("Time: " + (Math.Floor(current / 60) + (current % 60)/100).ToString("F2").Replace(",", ":") + " / " + (Math.Floor(length / 60) + (length % 60) / 100).ToString("F2").Replace(",", ":"));
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		//		RenderSettings.skybox = skyboxMaterialVideo
	}
}