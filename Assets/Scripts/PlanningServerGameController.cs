using UnityEngine;
using System.Collections;

public class PlanningServerGameController : BaseServerGameController {

	public const byte MESSAGE_SERVER_GAME_STATE = 5;
	public const byte MESSAGE_SERVER_PLAYER_ID = 6;

	new void Awake() {
		base.Awake ();
		SendGameState ();
		SendPlayerIds ();
	}

	void SendGameState() {
		byte commandByte = MESSAGE_SERVER_GAME_STATE;
		byte[] gameStateBytes = Utils.ObjectToBytes (networkManager.gameState);
		networkManager.SendDataAll(Utils.ConcatBytes(commandByte, gameStateBytes));
	}

	void SendPlayerIds() {
		foreach (int id in networkManager.connectionIds) {
			byte commandByte = MESSAGE_SERVER_PLAYER_ID;
			byte playerIdByte = (byte) id;
			networkManager.SendData(id, Utils.ConcatBytes(commandByte, playerIdByte));
		}
	}		
}
