﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ServerNetworkManager : SimpleServerNetworkManager {

	//Create a custom GameState class for your game.
	//GameState needs the [Serializable] Attribute, so that it can be synchronized with the clients.
	public GameState gameState;
	public List<int> connectionIds;
	private Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

	new void Awake() {
		base.Awake ();
		gameState = new GameState ();
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
		connectionIds.Remove (connectionId);
		return clients.Remove (connectionId);
	}



	public virtual void SendDataAll(byte data) {
		SendDataAll (new byte[]{ data });
	}

	public virtual void SendDataAll(byte[] data) {
		SendDataAll (data, data.Length);
	}

	public virtual void SendDataAll(byte[] data, int datasize) {
		foreach (int id in connectionIds) {
			SendData (id, data, datasize);
		}
	}
}
