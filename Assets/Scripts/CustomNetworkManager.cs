using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public abstract class CustomNetworkManager : MonoBehaviour {

	public int reliableChannelId;
	public int unreliableChannelId;
	public HostTopology topology;
	public int socketId;
	public int localPort;
	public int connectionId;
	public int connections;
	public bool isServer;

	public void Awake() {
		NetworkTransport.Init ();
	}		

	public virtual void ConnectAsClient(int socketIdPar, string externalIp, int externalPort) {
		isServer = false;
		if (socketIdPar == 0) {
			localPort = getAvailablePort ();
			Debug.Log ("Found free socket: " + localPort);
		} else {
			localPort = socketIdPar;
		}

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, 1);
		//TODO max connections on client?

		socketId = NetworkTransport.AddHost(topology, localPort);
		byte error;
		connectionId = NetworkTransport.Connect(socketId, externalIp, externalPort, 0, out error);
		if (((NetworkError)error) == NetworkError.Ok) {
			Debug.Log ("Connected as client to " + externalIp + ":" + externalPort);
		} else {
			Debug.Log ("Error connecting as client: " + ((NetworkError)error));
		}
	}

	public virtual void SetupAsServer(int socketId, int maxConnections) {
		isServer = true;

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, maxConnections);

		socketId = NetworkTransport.AddHost(topology, socketId);
		Debug.Log ("Set up server at port: " + socketId + " With a maximum of " + maxConnections + " Connections");
	}

	public virtual void SendData(byte[] data, int datasize) {
		byte error;
		NetworkTransport.Send (socketId, connectionId, reliableChannelId, data, datasize, out error);
		if (((NetworkError)error) != NetworkError.Ok) {
			Debug.Log ("Send error: " + ((NetworkError)error));
		}
	}

	void OnDisable() {
		//TODO move this to a different method?
		byte error;
		if (!isServer) {
			NetworkTransport.Disconnect (socketId, connectionId, out error);
			NetworkTransport.RemoveHost (socketId);
		}
	}

	private int getAvailablePort() {
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
			Debug.Log ("Receive error: " + ((NetworkError)error));
			Debug.Log ("ConnectionId: " + connectionId);
			Debug.Log ("RcvHostId: " + recHostId);
		} else {
			switch (recData) {
			case NetworkEventType.Nothing:
				RcvNothing(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.ConnectEvent:
				connections++;
				RcvConnect(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.DataEvent:
				RcvData(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.DisconnectEvent:
				connections--;
				RcvDisconnect(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			}
		}
	}

	public virtual void RcvNothing(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		Debug.Log ("NetworkEventType.Nothing");
	}

	public virtual void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		Debug.Log ("NetworkEventType.ConnectEvent");
	}

	public virtual void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		Debug.Log ("NetworkEventType.DataEvent");
	}

	public virtual void RcvDisconnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		Debug.Log ("NetworkEventType.DisconnectEvent");
	}
}
