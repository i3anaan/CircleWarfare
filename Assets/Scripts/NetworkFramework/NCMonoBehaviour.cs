using System;
using UnityEngine;


public class NCMonoBehaviour : MonoBehaviour, INetworkCallback
{
	public virtual void RcvNothing(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
	}

	public virtual void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
	}

	public virtual void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
	}

	public virtual void RcvDisconnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
	}
}

