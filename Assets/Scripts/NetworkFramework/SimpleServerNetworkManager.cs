using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public abstract class SimpleServerNetworkManager : BaseNetworkManager{

	public int reliableChannelId;
	public int unreliableChannelId;
	public HostTopology topology;
	public int localSocketId;

	public virtual void SetupAsServer(int socketId, int maxConnections) {
		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, maxConnections);

		localSocketId = NetworkTransport.AddHost(topology, socketId);
		Debug.Log ("Set up server at port: " + socketId + " With a maximum of " + maxConnections + " Connections");
	}

	public virtual void SendData(int connectionId, byte data) {
		SendData (connectionId, new byte[]{ data });
	}

	public virtual void SendData(int connectionId, byte[] data) {
		SendData (connectionId, data, data.Length);
	}

	public virtual void SendData(int connectionId, byte[] data, int datasize) {
		byte error;
		NetworkTransport.Send (localSocketId, connectionId, reliableChannelId, data, datasize, out error);
		if (((NetworkError)error) != NetworkError.Ok) {
			Debug.Log ("Send error: " + ((NetworkError)error));
		}
	}
}
