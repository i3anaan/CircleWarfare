using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClientNetworkManager : MonoBehaviour {

	private int reliableChannelId;
	private int unreliableChannelId;
	HostTopology topology;
	int hostId;
	private int connectionId;

	// Use this for initialization
	void Start () {
		NetworkTransport.Init ();

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, 4);

		hostId = NetworkTransport.AddHost(topology, 7788);

		byte error;
		byte[] buffer = new byte[]{1};
		int bufferLength = 1;
		connectionId = NetworkTransport.Connect(hostId, "130.89.183.126", 7777, 0, out error);
		Debug.Log (connectionId);
		NetworkTransport.Send(hostId, connectionId, reliableChannelId, buffer, bufferLength,  out error);
		Debug.Log ("Sending data!");
	}
}
