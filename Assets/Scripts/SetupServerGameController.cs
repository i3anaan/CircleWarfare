using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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

	void CheckAllReady() {
		bool allReady = true;
		foreach (int i in networkManager.connectionIds) {
			allReady = allReady && networkManager.GetClientData (i).ready;
		}

		if (allReady) {
			OnAllReady ();
		}
	}

	void OnAllReady() {
		//TODO Send ready signal to clients.
		SceneManager.LoadScene ("S_simulation");
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId) {
		playersConnected++;
		textField.text = playersConnected + "";

		ClientData client = new ClientData (connectionId);
		networkManager.AddClientData (client);
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId) {
		playersConnected--;
		textField.text = playersConnected + "";

		networkManager.RemoveClientData (connectionId);
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case MESSAGE_CLIENT_READY:
			GetClient (connectionId).ready = true;
			Debug.Log (GetClient (connectionId) + " is Ready!");
			CheckAllReady ();
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

	public override void RcvError(byte error, int rcvHostId, int connectionId, int channelId) {
		base.RcvError (error, rcvHostId, connectionId, channelId);
		switch ((NetworkError)error) {
		case NetworkError.Timeout:
			Debug.Log ("Timeout for: " + GetClient (connectionId));
			RcvDisconnect (rcvHostId, connectionId, channelId);
			break;
		default:
			//TODO Do nothing?
			break;
		}
	}
}
