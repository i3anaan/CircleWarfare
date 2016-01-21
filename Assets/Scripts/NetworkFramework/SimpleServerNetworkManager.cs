using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public abstract class SimpleServerNetworkManager : BaseNetworkManager{

	public int reliableChannelId;
	public int unreliableChannelId;
	public HostTopology topology;

	public virtual void SetupAsServer(int socketId, int maxConnections) {
		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, maxConnections);

		socketId = NetworkTransport.AddHost(topology, socketId);
		Debug.Log ("Set up server at port: " + socketId + " With a maximum of " + maxConnections + " Connections");
	}
}
