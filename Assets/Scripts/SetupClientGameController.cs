using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SetupClientGameController : BaseClientGameController {
	public Text connectedStatus;
	public Text address;
	public Text nameInputField;
	public Button ReadyButton;

	private float cooldown;
	public bool isReady;
	public bool isConnected;

	void FixedUpdate() {
		connectedStatus.text = isConnected ? "Connected!" : "Disconnected.";
		ReadyButton.gameObject.SetActive (isConnected);
		isReady = isConnected ? isReady : false;
	}

	public void OnClickReadyButton() {
		SendReadyMessage (!isReady);
		isReady = !isReady;
		ColorBlock colorBlock = ReadyButton.colors;
		colorBlock.normalColor = isReady ? Color.green : Color.white;
		colorBlock.highlightedColor = isReady ? Color.green : Color.white;
		ReadyButton.colors = colorBlock;
	}

	public void SendReadyMessage(bool ready) {
		networkManager.SendData (ready ? SetupServerGameController.MESSAGE_CLIENT_READY : SetupServerGameController.MESSAGE_CLIENT_NOT_READY);
	}

	private void SendName() {
		byte commandByte = SetupServerGameController.MESSAGE_CLIENT_NAME;
		string name = nameInputField.text;
		byte[] nameBytes = Utils.StringToBytes (name);
		byte[] bytes = Utils.ConcatBytes (commandByte, nameBytes);
		networkManager.SendData(bytes); 
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId) {
		isConnected = true;
		address.text = "[0.0.0.0]:" + networkManager.localPort;
		SendName ();
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId) {
		isConnected = false;
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case SetupServerGameController.MESSAGE_SERVER_NEXT_PHASE_PLANNING:
			SceneManager.LoadScene ("C_planning");
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
			RcvDisconnect (rcvHostId, connectionId, channelId);
			break;
		default:
			//Do nothing
			break;
		}
	}
}
