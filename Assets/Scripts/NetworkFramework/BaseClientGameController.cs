using System;
using UnityEngine;
using UnityEngine.Networking;

public class BaseClientGameController : NCMonoBehaviour
{
	public ClientNetworkManager networkManager;

	void Start() {
		OnLevelWasLoaded (-1);
	}

	void OnLevelWasLoaded(int level) {
		networkManager = (ClientNetworkManager) GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ClientNetworkManager>();
	}

	public bool CheckConnection() {
		byte error;
		NetworkTransport.GetCurrentRtt (networkManager.localSocketId, networkManager.connectionId, out error);
		Debug.Log (((NetworkError)error));
		return ((NetworkError)error) == NetworkError.Ok;
	}
}

