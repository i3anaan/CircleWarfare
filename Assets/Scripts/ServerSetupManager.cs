using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerSetupManager : CustomNetworkManager {

	private int playersConnected;
	public Text textField;

	// Use this for initialization
	void Start () {
		SetupAsServer (7522, 8);
		playersConnected = 0;
	}

	void Update() {
		base.Update ();
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("Server");
		}
	}
		
	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		playersConnected++;
		textField.text = playersConnected + "";
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		base.RcvDisconnect (rcvHostId, connectionId, channelId, rcvBuffer, datasize);
		playersConnected--;
		textField.text = playersConnected + "";
	}
}
