using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CustomNetworkController : NetworkManager {

	public GameObject clientUI;
	public GameObject serverUI;
	public TeamPriorities priorities;

	public override void OnStartServer() {
		Debug.Log ("OnStartServer()");
		serverUI.SetActive (true);
	}

	public override void OnStartClient(NetworkClient client) {
		Debug.Log ("OnStartClient()");
		clientUI.SetActive (true);
	}

	public override void OnServerConnect(NetworkConnection conn) {
		Debug.Log ("Server: A Client connected!");
	}

	public override void OnClientConnect(NetworkConnection conn) {
		Debug.Log ("Client: Connected to a server!");
	}

	public void InformServerOfNewPriorities() {
		Debug.Log ("InformServer()");
		((NetworkPlayer)NetworkPlayer.getPlayerObject ()).priorities = this.priorities;
		((NetworkPlayer)NetworkPlayer.getPlayerObject ()).CmdInformServer ();
	}
}
