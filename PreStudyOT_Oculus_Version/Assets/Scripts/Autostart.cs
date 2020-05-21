using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Discovery
{
	public class Autostart : MonoBehaviour
	{
		public NetworkDiscovery networkDiscovery;
		// Start is called before the first frame update
		void Start()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
				NetworkManager.singleton.StartHost();
				networkDiscovery.AdvertiseServer();
			#endif
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}