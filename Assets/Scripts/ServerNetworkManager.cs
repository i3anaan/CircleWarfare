using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerNetworkManager : MonoBehaviour {

	private int reliableChannelId;
	private int unreliableChannelId;
	HostTopology topology;
	int socketId;

	// Use this for initialization
	void Start () {
		NetworkTransport.Init ();

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, 4);

		socketId = NetworkTransport.AddHost(topology, 7522);
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
		Debug.Log ("Error: " + ((NetworkError) error));

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
