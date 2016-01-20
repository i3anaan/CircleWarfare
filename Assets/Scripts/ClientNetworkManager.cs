using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientNetworkManager : CustomNetworkManager {

	public Text connectedStatus;
	public Text address;

	private float cooldown;

	void Connect() {
		ConnectAsClient (0, "0.0.0.0", 7522);
	}		

	void FixedUpdate() {
		cooldown += Time.fixedDeltaTime;
		if (cooldown >= 1) {
			cooldown = 0;
			SendData (new byte[]{ 1 }, 1);
		}
		connectedStatus.text = CheckConnection() ? "Connected!" : "Disconnected.";
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		address.text = "[0.0.0.0]:" + localPort;
	}

	private bool CheckConnection() {
		byte error;
		NetworkTransport.GetCurrentRtt (socketId, connectionId, out error);
		Debug.Log (((NetworkError)error));
		return ((NetworkError)error) == NetworkError.Ok;
	}
}
