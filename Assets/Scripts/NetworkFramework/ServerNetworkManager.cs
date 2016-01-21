using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerNetworkManager : SimpleServerNetworkManager {

	new void Awake() {
		base.Awake ();
		SetupAsServer (7522, 8);
	}

	void Start() {
		OnLevelWasLoaded (-1);
	}

	void OnLevelWasLoaded(int level) {
		networkCallback = (INetworkCallback) GameObject.FindGameObjectWithTag("GameController").GetComponent<INetworkCallback>();
	}
}
