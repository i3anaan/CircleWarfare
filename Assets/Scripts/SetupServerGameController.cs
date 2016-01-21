using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupServerGameController : NCMonoBehaviour {

	private int playersConnected;
	public Text textField;

	void Start () {
		playersConnected = 0;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("S_planning");
		}
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		playersConnected++;
		textField.text = playersConnected + "";
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		playersConnected--;
		textField.text = playersConnected + "";
	}
}
