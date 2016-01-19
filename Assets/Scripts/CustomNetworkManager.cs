using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class CustomNetworkManager : MonoBehaviour {

	public int reliableChannelId;
	public int unreliableChannelId;
	public HostTopology topology;
	public int socketId;
	public int connectionId;

	public virtual void ConnectAsClient(int socketId, string externalIp, int externalPort) {
		NetworkTransport.Init ();

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, 1);
		//TODO max connections on client?

		socketId = NetworkTransport.AddHost(topology, socketId);
		byte error;
		connectionId = NetworkTransport.Connect(socketId, externalIp, externalPort, 0, out error);
		if (((NetworkError)error) == NetworkError.Ok) {
			Debug.Log ("Connected as client to " + externalIp + ":" + externalPort);
		} else {
			Debug.Log ("Error connecting as client: " + ((NetworkError)error));
		}
	}

	public virtual void SetupAsServer(int socketId, int maxConnections) {
		NetworkTransport.Init ();

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
		NetworkTransport.RemoveHost (socketId);
	}

	void Update() {
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
		} else {
			switch (recData) {
			case NetworkEventType.Nothing:
				RcvNothing(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.ConnectEvent:
				RcvConnect(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.DataEvent:
				RcvData(recHostId, connectionId, channelId, recBuffer, dataSize);
				break;
			case NetworkEventType.DisconnectEvent:
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
