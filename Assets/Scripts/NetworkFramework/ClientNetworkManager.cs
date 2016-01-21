using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientNetworkManager : SimpleClientNetworkManager {

	public void Connect() {
		ConnectAsClient (0, "0.0.0.0", 7522);
	}

	void Start() {
		OnLevelWasLoaded (-1);
	}

	void OnLevelWasLoaded(int level) {
		networkCallback = (INetworkCallback) GameObject.FindGameObjectWithTag("GameController").GetComponent<INetworkCallback>();
	}
}
