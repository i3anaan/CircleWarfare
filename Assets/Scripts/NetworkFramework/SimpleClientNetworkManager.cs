using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public abstract class SimpleClientNetworkManager : BaseNetworkManager {

	public int reliableChannelId;
	public int unreliableChannelId;
	public HostTopology topology;
	public int localSocketId;
	public int localPort;
	public int connectionId;

	public virtual void ConnectAsClient(int socketIdPar, string externalIp, int externalPort) {
		if (socketIdPar == 0) {
			localPort = getAvailablePort ();
			//Debug.Log ("Found free socket: " + localPort);
		} else {
			localPort = socketIdPar;
		}

		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		unreliableChannelId = config.AddChannel(QosType.Unreliable);

		topology = new HostTopology(config, 1);
		//TODO max connections on client?

		localSocketId = NetworkTransport.AddHost(topology, localPort);
		byte error;
		connectionId = NetworkTransport.Connect(localSocketId, externalIp, externalPort, 0, out error);
		if (((NetworkError)error) == NetworkError.Ok) {
			Debug.Log ("Connected as client to " + externalIp + ":" + externalPort);
		} else {
			Debug.Log ("Error connecting as client: " + ((NetworkError)error));
		}
	}

	void OnDisable() {
		//TODO throws error (no big deal though).
		byte error;
		NetworkTransport.Disconnect (localSocketId, connectionId, out error);
		NetworkTransport.RemoveHost (localSocketId);
	}

	public virtual void SendData(byte data) {
		SendData (new byte[]{ data });
	}

	public virtual void SendData(byte[] data) {
		SendData (data, data.Length);
	}

	public virtual void SendData(byte[] data, int datasize) {
		byte error;
		NetworkTransport.Send (localSocketId, connectionId, reliableChannelId, data, datasize, out error);
		if (((NetworkError)error) != NetworkError.Ok) {
			Debug.Log ("Send error: " + ((NetworkError)error));
		}
	}
}
