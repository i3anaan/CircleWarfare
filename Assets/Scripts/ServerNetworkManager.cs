using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerNetworkManager : CustomNetworkManager {

	public GameController gc;

	void Start () {
		SetupAsServer (7522, 4);
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		gc.StartGame ();
	}
}
