using System;
using UnityEngine;
using UnityEngine.Networking;

public class BaseClientGameController : NCMonoBehaviour
{
	public ClientNetworkManager networkManager;

	public void Awake() {
		networkManager = (ClientNetworkManager) GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ClientNetworkManager>();
	}
}

