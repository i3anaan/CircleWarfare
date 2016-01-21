using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupClientGameController : BaseClientGameController {
	public Text connectedStatus;
	public Text address;
	public Text nameInputField;
	public Button ReadyButton;

	private float cooldown;
	public bool isReady;

	void FixedUpdate() {
		cooldown += Time.fixedDeltaTime;
		if (cooldown >= 1) {
			cooldown = 0;
			//networkManager.SendData (new byte[]{ 1 }, 1);
		}
		connectedStatus.text = CheckConnection() ? "Connected!" : "Disconnected.";
		ReadyButton.gameObject.SetActive (CheckConnection ());
	}

	public override void RcvConnect(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
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
}
