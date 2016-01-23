﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public abstract class BaseNetworkManager : MonoBehaviour, INetworkCallback{

	public INetworkCallback networkCallback;

	public void Awake() {
		DontDestroyOnLoad (transform.gameObject);
		NetworkTransport.Init ();
	}

	protected int getAvailablePort() {
		//From:
		//http://forum.unity3d.com/threads/workaround-how-to-get-an-available-port-to-start-a-unet-host.371644/
		var address = IPAddress.Parse("0.0.0.0");
		IPEndPoint endpoint;
		using (var tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
			tempSocket.Bind(new IPEndPoint(address, port: 0));
			var availablePort = ((IPEndPoint) tempSocket.LocalEndPoint).Port;
			endpoint = new IPEndPoint(address , availablePort);
		}

		return endpoint.Port;
	}

	public void Update() {
		int recHostId; 
		int connectionId; 
		int channelId; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
		if (((NetworkError)error) != NetworkError.Ok) {
			RcvError (error, recHostId, connectionId, channelId);
		} else {
			switch (recData) {
			case NetworkEventType.Nothing:
				RcvNothing(recHostId);
				break;
			case NetworkEventType.ConnectEvent:
				RcvConnect(recHostId, connectionId, channelId);
				break;
			case NetworkEventType.DataEvent:
				RcvData(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.DisconnectEvent:
				RcvDisconnect(recHostId, connectionId, channelId);
				break;
			}
		}
	}

	public virtual void RcvNothing(int rcvHostId) {
		if (networkCallback != null) {
			networkCallback.RcvNothing (rcvHostId);
		}
	}

	public virtual void RcvConnect(int rcvHostId, int connectionId, int channelId) {
		if (networkCallback != null) {
			networkCallback.RcvConnect (rcvHostId, connectionId, channelId);
		}
	}

	public virtual void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		if (networkCallback != null) {
			networkCallback.RcvData (rcvHostId, connectionId, channelId, rcvBuffer, datasize);
		}
	}

	public virtual void RcvDisconnect(int rcvHostId, int connectionId, int channelId) {
		if (networkCallback != null) {
			networkCallback.RcvDisconnect (rcvHostId, connectionId, channelId);
		}
	}

	public virtual void RcvError(byte error, int rcvHostId, int connectionId, int channelId) {
		if (networkCallback != null) {
			networkCallback.RcvError (error, rcvHostId, connectionId, channelId);
		}
	}
}