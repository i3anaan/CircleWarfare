using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupServerGameController : BaseServerGameController {

	public const byte MESSAGE_CLIENT_READY = 1;
	public const byte MESSAGE_CLIENT_NOT_READY = 2;
	public const byte MESSAGE_CLIENT_NAME = 3;

	private int playersConnected;
	public Text textField;

	new void Start () {
		base.Start ();
		playersConnected = 0;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("S_simulation");
		}
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		playersConnected++;
		textField.text = playersConnected + "";

		ClientData client = new ClientData (connectionId);
		networkManager.AddClientData (client);
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		playersConnected--;
		textField.text = playersConnected + "";

		networkManager.RemoveClientData (connectionId);
	}


	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case MESSAGE_CLIENT_READY:
			GetClient (connectionId).ready = true;
			Debug.Log (GetClient (connectionId) + " is Ready!");
			break;
		case MESSAGE_CLIENT_NOT_READY:
			GetClient (connectionId).ready = false;
			Debug.Log (GetClient (connectionId) + " is no longer Ready!");
			break;
		case MESSAGE_CLIENT_NAME:
			string name = BaseNetworkManager.BytesToString (rcvBuffer, 1, datasize - 1);
			GetClient (connectionId).name = name;
			break;			
		default:
			//Do nothing?
			break;
		}			
	}
}
