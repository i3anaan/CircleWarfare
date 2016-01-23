using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlanningServerGameController : BaseServerGameController {

	public float planningPhaseDuration;
	private int planningPhaseTicks;

	public const byte MESSAGE_SERVER_GAME_STATE = 5;
	public const byte MESSAGE_SERVER_PLAYER_ID = 6;
	public const byte MESSAGE_SERVER_NEXT_PHASE_SIMULATION = 8;
	public const byte MESSAGE_CLIENT_GAME_PRIORITY = 7;

	new void Awake() {
		base.Awake ();
		SendGameState ();
		SendPlayerIds ();

	}

	void FixedUpdate() {
		if (planningPhaseTicks > planningPhaseDuration / Time.fixedDeltaTime) {
			SendNextPhase ();
			SceneManager.LoadScene ("S_simulation");
		}

		planningPhaseTicks++;
	}

	void SendGameState() {
		byte commandByte = MESSAGE_SERVER_GAME_STATE;
		byte[] gameStateBytes = Utils.ObjectToBytes (networkManager.gameState);
		networkManager.SendDataAll(Utils.ConcatBytes(commandByte, gameStateBytes));
	}

	void SendNextPhase() {
		byte commandByte = MESSAGE_SERVER_NEXT_PHASE_SIMULATION;
		networkManager.SendDataAll (commandByte);
	}

	void SendPlayerIds() {
		foreach (int id in networkManager.connectionIds) {
			byte commandByte = MESSAGE_SERVER_PLAYER_ID;
			byte playerIdByte = (byte) id;
			networkManager.SendData(id, Utils.ConcatBytes(commandByte, playerIdByte));
		}
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case MESSAGE_CLIENT_GAME_PRIORITY:
			byte[] bytes = Utils.SubArray (rcvBuffer, 1, datasize);
			float[] priorities = (float[]) Utils.BytesToObject (bytes);
			UpdatePlayerPriorities (connectionId, priorities);
			break;
		default:
			Debug.LogError ("Received unkown command!");
			break;
		}			
	}

	private void UpdatePlayerPriorities(int connectionId, float[] priorities) {
		networkManager.GetClientData (connectionId).priorities = priorities;
	}
}
