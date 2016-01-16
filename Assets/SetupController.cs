using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SetupController : NetworkBehaviour {
	public int clientCount;

	[Server]
	void Awake() {
		NetworkServer.Spawn (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		if (NetworkServer.active && NetworkClient.active) {
			Debug.LogError ("This process is both a server and client, which is unsupported by this game!");
		} else if (NetworkServer.active) {
			Debug.Log ("isServer");
		} else if (NetworkClient.active) {
			Debug.Log ("isClient");
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("Player " + clientCount + " connected from " + player.ipAddress + ":" + player.port);
	}
	
	void OnConnectedToServer() {
		Debug.Log("Connected to server");
	}

	void OnNetworkInstantiate(NetworkMessageInfo info) {
		Debug.Log("New object instantiated by " + info.sender);
	}
	void OnServerInitialized() {
		Debug.Log("Server initialized and ready");
	}
}
