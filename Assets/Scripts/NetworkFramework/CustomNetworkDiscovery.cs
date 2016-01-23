using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CustomNetworkDiscovery : NetworkDiscovery {

	private ClientNetworkManager client;

	void Start() {
		this.Initialize ();
		GameObject[] clients = GameObject.FindGameObjectsWithTag ("NetworkManager");
		client = clients[clients.Length-1].GetComponent<ClientNetworkManager> ();
		//If the first scene is reloaded, for a moment 2 ClientNetworkManagers exist.
		//The oldest one will then delete itself, but before doing so, this will already look for NetworkManagers.
		//Thus the solution is to always link the youngest (last) ClientNetworkManager.

		if (client == null) {
			this.StartAsServer ();
		}
	}
		

	public override void OnReceivedBroadcast(string fromAddress, string data) {
		//TODO Check data?
		if (client != null) {
			client.Connect (fromAddress);
			GameObject.Destroy (this.gameObject);
		}
	}
}
