using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class SetupServerGameController : BaseServerGameController {

	public const byte MESSAGE_CLIENT_READY = 1;
	public const byte MESSAGE_CLIENT_NOT_READY = 2;
	public const byte MESSAGE_CLIENT_NAME = 3;
	public const byte MESSAGE_SERVER_NEXT_PHASE_PLANNING = 4;

	public Text startCountdownField;

	private int playersConnected;
	public Text textField;

	public int readyDelay = 5;
	private int ticksTillStart = -1;

	void Start () {
		playersConnected = 0;
	}

	void FixedUpdate() {
		if (ticksTillStart == 0) {
			networkManager.SendDataAll (MESSAGE_SERVER_NEXT_PHASE_PLANNING);
			networkManager.gameState.teams = networkManager.connectionIds.Count;
			SceneManager.LoadScene ("S_planning");
		} else if (ticksTillStart > 0) {
			ticksTillStart--;
			startCountdownField.text = "Game starts in: " + (ticksTillStart * Time.fixedDeltaTime);
		}
	}

	void CheckReady() {
		bool allReady = true;
		foreach (int i in networkManager.connectionIds) {
			allReady = allReady && networkManager.GetClientData (i).ready;
		}

		if (allReady) {
			if (networkManager.connectionIds.Count > 1) {
				OnAllReady ();
			} else {
				startCountdownField.text = "This game requires atleast 2 players!";
			}				
		} else {
			OnNotAllReady ();
		}
	}

	void OnAllReady() {
		ticksTillStart = (int) (readyDelay / Time.fixedDeltaTime);
	}

	void OnNotAllReady() {
		ticksTillStart = -1;
		startCountdownField.text = "Ready up to start the game!";
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
			CheckReady ();
			break;
		case MESSAGE_CLIENT_NOT_READY:
			GetClient (connectionId).ready = false;
			Debug.Log (GetClient (connectionId) + " is no longer Ready!");
			CheckReady ();
			break;
		case MESSAGE_CLIENT_NAME:
			string name = Utils.BytesToString (rcvBuffer, 1, datasize - 1);
			GetClient (connectionId).name = name;
			break;			
		default:
			Debug.LogError ("Received unkown command!");
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
			//Do nothing
			break;
		}
	}
}
