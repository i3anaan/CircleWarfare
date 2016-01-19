using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClientNetworkManager : MonoBehaviour {

	private int reliableChannelId;
	private int unreliableChannelId;
	HostTopology topology;
	int SocketId;
	private int connectionId;
	private int wait;


	void Awake() {
		NetworkTransport.Init ();

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, 4);

		SocketId = NetworkTransport.AddHost(topology, 7523);
		byte error;
		connectionId = NetworkTransport.Connect(SocketId, "0.0.0.0", 7522, 0, out error);
		Debug.Log ("Error: " + ((NetworkError) error));
		Debug.Log ("ConnectionID: " + connectionId);
	}

	void FixedUpdate() {
		byte error;
		byte[] buffer = new byte[]{ 1 };
		int bufferLength = 1;
		NetworkTransport.Send (SocketId, connectionId, reliableChannelId, buffer, bufferLength, out error);
		Debug.Log ("Send Error: " + ((NetworkError) error));
		Debug.Log ("Sending data!");			
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
		Debug.Log ("Receive Error: " + ((NetworkError) error));

		switch (recData)
		{
		case NetworkEventType.Nothing:         //1
			Debug.Log("NetworkEventType.Nothing");
			break;
		case NetworkEventType.ConnectEvent:    //2
			Debug.Log("NetworkEventType.ConnectEvent");
			break;
		case NetworkEventType.DataEvent:       //3
			Debug.Log("NetworkEventType.DataEvent");
			break;
		case NetworkEventType.DisconnectEvent: //4
			Debug.Log("NetworkEventType.DisconnectEvent");
			break;
		}
	}
}
