using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ClientServerNetworkManager : MonoBehaviour {
	
	private int myReliableChannelId;

	int socketId;
	int socketPort = 8888;

	int connectionId;


	public void Start() {
		NetworkTransport.Init();
		ConnectionConfig config = new ConnectionConfig();

		myReliableChannelId = config.AddChannel(QosType.Reliable);
		int maxConnections = 4;
		HostTopology topology = new HostTopology(config, maxConnections);

		socketId = NetworkTransport.AddHost(topology, socketPort);
		Debug.Log("Socket Open. SocketId is: " + socketId);

		Connect ();
	}

	public void Connect() {
		byte error;
		connectionId = NetworkTransport.Connect(socketId, "0.0.0.0", socketPort, 0, out error);
		Debug.Log("Connected to server. ConnectionId: " + connectionId);
	}

	public void SendSocketMessage() {
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, "HelloServer");

		int bufferSize = 1024;

		NetworkTransport.Send(socketId, connectionId, myReliableChannelId, buffer, bufferSize, out error);
	}

	void Update() {
		int recHostId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recNetworkEvent = NetworkTransport.Receive (out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

		switch (recNetworkEvent) {
		case NetworkEventType.Nothing:
			break;
		case NetworkEventType.ConnectEvent:
			Debug.Log ("incoming connection event received");
			break;
		case NetworkEventType.DataEvent:
			Stream stream = new MemoryStream (recBuffer);
			BinaryFormatter formatter = new BinaryFormatter ();
			string message = formatter.Deserialize (stream) as string;
			Debug.Log ("incoming message event received: " + message);
			break;
		case NetworkEventType.DisconnectEvent:
			Debug.Log ("remote client event disconnected");
			break;
		}
	}

	public void FixedUpdate() {
		SendSocketMessage ();
	}
}
