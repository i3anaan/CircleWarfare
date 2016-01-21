﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ServerNetworkManager : SimpleServerNetworkManager {

	public List<int> connectionIds;
	private Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

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

	public void AddClientData(ClientData client) {
		clients.Add (client.connectionId, client);
		connectionIds.Add (client.connectionId);
	}

	public ClientData GetClientData(int connectionId) {
		ClientData client = null;
		clients.TryGetValue (connectionId, out client);
		return client;
	}

	public bool RemoveClientData(int connectionId) {
		return clients.Remove (connectionId);
	}		
}
