using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlanningServerGameController : BaseServerGameController {

	public float planningPhaseDuration;
	private int planningPhaseTicks;
	private int previousPhaseCountDown = -1;
	public TextSplasher splasher;

	public const byte MESSAGE_SERVER_GAME_STATE = 5;
	public const byte MESSAGE_SERVER_PLAYER_ID = 6;
	public const byte MESSAGE_SERVER_NEXT_PHASE_SIMULATION = 8;
	public const byte MESSAGE_SERVER_CLIENT_DATA = 11;
	public const byte MESSAGE_CLIENT_GAME_PRIORITY = 7;

	new void Awake() {
		base.Awake ();

		for (int i = 0; i < networkManager.gameState.teams; i++) {
			networkManager.gameState.names [i] = GetClient (i + 1).name;
		}

		SendClientData ();
		SendGameState ();
		SendPlayerIds ();
	}

	void FixedUpdate() {
		if (planningPhaseTicks > planningPhaseDuration / Time.fixedDeltaTime) {
			SendNextPhase ();
			SceneManager.LoadScene ("S_simulation");
		}
		int countdown = ((int) (planningPhaseDuration - planningPhaseTicks * Time.fixedDeltaTime + 1f));
		planningPhaseTicks++;
		if (previousPhaseCountDown != countdown) {
			splasher.Splash (countdown + "", 1, 1, Color.red);
			previousPhaseCountDown = countdown;
		}
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

	void SendClientData() {
		byte commandByte = MESSAGE_SERVER_CLIENT_DATA;
		foreach (int connId in networkManager.connectionIds) {
			byte[] clientDataBytes = Utils.ObjectToBytes (networkManager.GetClientData(connId));
			networkManager.SendData(connId, Utils.ConcatBytes(commandByte, clientDataBytes));
		}
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
