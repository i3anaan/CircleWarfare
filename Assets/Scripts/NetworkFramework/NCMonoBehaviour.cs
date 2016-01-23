using System;
using UnityEngine;
using UnityEngine.Networking;


public class NCMonoBehaviour : MonoBehaviour, INetworkCallback
{
	public virtual void RcvNothing(int rcvHostId) {
	}

	public virtual void RcvConnect(int rcvHostId, int connectionId, int channelId) {
	}

	public virtual void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
	}

	public virtual void RcvDisconnect(int rcvHostId, int connectionId, int channelId) {
	}

	public virtual void RcvError(byte error, int rcvHostId, int connectionId, int channelId) {
		Debug.Log ("Receive error: " + ((NetworkError)error));
		Debug.Log ("ConnectionId: " + connectionId);
		Debug.Log ("RcvHostId: " + rcvHostId);
	}
}

