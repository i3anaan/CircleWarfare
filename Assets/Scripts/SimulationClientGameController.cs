using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SimulationClientGameController : BaseClientGameController {

	private int cooldown;

	// Use this for initialization
	void FixedUpdate () {
		if (cooldown > 1 / Time.fixedDeltaTime) {
			networkManager.SendData ((byte)1);
			cooldown = 0;
		}			

		cooldown++;
	}

	public override void RcvDisconnect(int rcvHostId, int connectionId, int channelId) {
		SceneManager.LoadScene ("C_setup");
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
