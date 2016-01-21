using System;
public interface INetworkCallback
{
	void RcvNothing(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize);

	void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize);

	void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize);

	void RcvDisconnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize);
}

