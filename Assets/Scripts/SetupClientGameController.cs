using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupClientGameController : BaseClientGameController {
	public Text connectedStatus;
	public Text address;

	private float cooldown;

	void FixedUpdate() {
		cooldown += Time.fixedDeltaTime;
		if (cooldown >= 1) {
			cooldown = 0;
			networkManager.SendData (new byte[]{ 1 }, 1);
		}
		connectedStatus.text = CheckConnection() ? "Connected!" : "Disconnected.";
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		address.text = "[0.0.0.0]:" + networkManager.localPort;
	}
}
