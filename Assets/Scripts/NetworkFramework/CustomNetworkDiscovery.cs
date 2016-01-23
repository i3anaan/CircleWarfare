using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CustomNetworkDiscovery : NetworkDiscovery {

	private ClientNetworkManager client;

	void Start() {
		client = (ClientNetworkManager) GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ClientNetworkManager>();

		this.Initialize ();

		if (client == null) {
			this.StartAsServer ();
		}
	}

	public override void OnReceivedBroadcast(string fromAddress, string data) {
		Debug.Log ("Received broadcast from: " + fromAddress);

		//TODO Check data?
		if (client != null) {
			client.Connect (fromAddress);
			GameObject.Destroy (this.gameObject);
		}
	}
}
