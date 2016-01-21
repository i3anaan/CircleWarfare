﻿using System;
using UnityEngine;
using UnityEngine.Networking;

public class BaseClientGameController : NCMonoBehaviour
{
	public ClientNetworkManager networkManager;

	void Awake() {
		networkManager = (ClientNetworkManager) GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ClientNetworkManager>();
	}
}

