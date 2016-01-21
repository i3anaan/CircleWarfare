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

	void FixedUpdate() {
		connectedStatus.text = CheckConnection() ? "Connected!" : "Disconnected.";
		ReadyButton.gameObject.SetActive (CheckConnection ());
		isReady = CheckConnection() ? isReady : false;
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId) {
		address.text = "[0.0.0.0]:" + networkManager.localPort;

		SendName ();
	}

	public void OnClickReadyButton() {
		SendReadyMessage (!isReady);
		isReady = !isReady;
	}

	public void SendReadyMessage(bool ready) {
		networkManager.SendData (ready ? SetupServerGameController.MESSAGE_CLIENT_READY : SetupServerGameController.MESSAGE_CLIENT_NOT_READY);
	}

	private void SendName() {
		byte commandByte = SetupServerGameController.MESSAGE_CLIENT_NAME;
		string name = nameInputField.text;
		byte[] nameBytes = BaseNetworkManager.StringToBytes (name);
		byte[] bytes = BaseNetworkManager.ConcatBytes (commandByte, nameBytes);

		networkManager.SendData(bytes); 
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case SetupServerGameController.MESSAGE_SERVER_GAME_START:
			SceneManager.LoadScene ("C_planning");
			break;
		default:
			Debug.LogError ("Received unkown command!");
			break;
		}			
	}
}
