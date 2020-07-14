using UnityEngine;
using Mirror;
public class NetworkControler : NetworkManager
{

	[Tooltip("Prefab of the controler object. Prefab must have a Network Identity component?")]
	public GameObject controlerPrefab;
	public bool runWithoutOculus = false; //debug to use the GuiControl on the Host

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		Transform startPos = GetStartPosition();
		GameObject player;
		if (!runWithoutOculus && this.mode == NetworkManagerMode.Host && this.numPlayers == 0)
		{
			player = startPos != null
				? Instantiate(playerPrefab, startPos.position, startPos.rotation)
				: Instantiate(playerPrefab);

		}
		else
		{
			player = startPos != null
				? Instantiate(controlerPrefab, startPos.position, startPos.rotation)
				: Instantiate(controlerPrefab);
		}
		NetworkServer.AddPlayerForConnection(conn, player);
	}
}
