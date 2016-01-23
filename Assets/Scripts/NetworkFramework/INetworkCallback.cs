using System;
public interface INetworkCallback
{
	void RcvNothing(int rcvHostId);

	void RcvConnect(int rcvHostId, int connectionId, int channelId);

	void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize);

	void RcvDisconnect(int rcvHostId, int connectionId, int channelId);

	void RcvError (byte error, int rcvHostId, int connectionId, int channelId);
}

