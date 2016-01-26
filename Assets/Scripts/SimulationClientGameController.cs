using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SimulationClientGameController : BaseClientGameController {

	private int cooldown;
	public SpriteRenderer background;
	public TextSplasher splasher;

	public override void Awake() {
		base.Awake ();
		int playerId = networkManager.clientData.playerId;
		background.color = Color.Lerp(SimulationServerGameController.colors [playerId - 1], Color.white, 0.5f);
	}

	void FixedUpdate () {
		if (cooldown > 1 / Time.fixedDeltaTime) {
			networkManager.SendData ((byte)1);
			cooldown = 0;
		}			

		cooldown++;
	}

	private void ShowWin() {
		splasher.Splash ("YOU WIN!", 10, 1, Color.red);
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId) {
		SceneManager.LoadScene ("C_setup");
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case SimulationServerGameController.MESSAGE_SERVER_GAME_OVER:
			int winner = (int)rcvBuffer [1];
			if (winner == networkManager.clientData.playerId) {
				ShowWin ();
			}
			break;
		case SimulationServerGameController.MESSAGE_SERVER_NEXT_PHASE_PLANNING:
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
