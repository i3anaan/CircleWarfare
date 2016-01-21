using System;
using UnityEngine.Networking;
using UnityEngine;

public class BaseServerGameController : NCMonoBehaviour
{
	public ServerNetworkManager networkManager;

	public virtual void Start() {
		OnLevelWasLoaded (-1);
	}

	void OnLevelWasLoaded(int level) {
		networkManager = (ServerNetworkManager) GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ServerNetworkManager>();
	}



	protected ClientData GetClient(int connectionId) {
		//Shortcut method.
		return networkManager.GetClientData (connectionId);
	}
}

