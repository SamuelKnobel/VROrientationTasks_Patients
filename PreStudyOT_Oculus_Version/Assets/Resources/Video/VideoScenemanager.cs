using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VideoScenemanager : NetworkBehaviour
{
	public GameObject videoPlayerPrefab;
    // Start is called before the first frame update
    void Start()
    {
		CmdAddVideoManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	[Command]
	void CmdAddVideoManager()
	{
		GameObject videoPlayer = Instantiate(videoPlayerPrefab);
		NetworkServer.Spawn(videoPlayer);
	}
}
