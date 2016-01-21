using UnityEngine;
using System.Collections;

public class PlanningClientGameController : BaseClientGameController {

	private GameState gameState;
	public int playerId;

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case PlanningServerGameController.MESSAGE_SERVER_GAME_STATE:
			byte[] bytes = Utils.SubArray (rcvBuffer, 1, datasize);
			GameState gs = (GameState) Utils.BytesToObject (bytes);
			networkManager.gameState = gs;
			Debug.Log ("Received GameState: " + gs);
			break;
		case PlanningServerGameController.MESSAGE_SERVER_PLAYER_ID:
			this.playerId = (int)rcvBuffer [1];
			Debug.Log ("Received player id: " + playerId);
			break;
		default:
			Debug.LogError ("Received unkown command!");
			break;
		}			
	}
}
