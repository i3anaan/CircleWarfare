using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientNetworkManager : SimpleClientNetworkManager {
	
	//Create a custom GameState class for your game.
	//GameState needs the [Serializable] Attribute, so that it can be synchronized with the clients.
	public GameState gameState;
	public CustomNetworkDiscovery networkDiscovery;

	public void ConnectLocalNetwork() {
		networkDiscovery.StartAsClient ();
	}

	public void Connect(string address) {
		//Debug.Log ("Connecting to: " + address);
		ConnectAsClient (0, address, 7522);
	}

	public void ConnectLocalHost() {
		ConnectAsClient (0, "0.0.0.0", 7522);
	}

	void Start() {
		OnLevelWasLoaded (-1);
	}

	public override void OnLevelWasLoaded(int level) {
		base.OnLevelWasLoaded (level);
		networkCallback = (INetworkCallback) GameObject.FindGameObjectWithTag("GameController").GetComponent<INetworkCallback>();
	}
}
