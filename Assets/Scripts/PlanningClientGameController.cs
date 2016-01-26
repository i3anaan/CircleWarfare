using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class PlanningClientGameController : BaseClientGameController {
	
	public Slider sliderPrefab;
	public Canvas canvas;

	private List<Slider> sliders;
	private bool slidersDirty;
	private int prioritySyncCooldown;

	private GameState gameState;
	public int playerId;
	public SpriteRenderer background;

	private void InitializeUI() {
		background.color = Color.Lerp(SimulationServerGameController.colors [playerId - 1], Color.white, 0.5f);
		sliders = new List<Slider> ();
		int sliderCount = networkManager.gameState.teams;
		for (int i = 0; i < sliderCount; i++) {
			Vector3 sliderPos = new Vector3 (-(sliderCount / 2) * 300 + i * 300, 0, 0);
			Slider slider = (Slider) GameObject.Instantiate (sliderPrefab, sliderPos, Quaternion.identity);
			sliders.Add (slider);
			slider.onValueChanged.AddListener (delegate {SliderChanged ();});
			slider.transform.SetParent (canvas.transform, false);
			slider.transform.localScale = new Vector3 (1, 1, 1);
			slider.value = networkManager.clientData.priorities [i];

			//Set slider top-part color
			slider.transform.GetChild (0).gameObject.GetComponent<Image> ().color = SimulationServerGameController.colors [i];
			//Set slider bottom-part color
			slider.transform.GetChild (1).GetChild(0).gameObject.GetComponent<Image> ().color = SimulationServerGameController.colors [i];

			if (i == playerId - 1) {
				slider.interactable = false;
			}
		}

		syncPriorityValues ();
	}

	public void SliderChanged() {
		slidersDirty = true;
	}

	public void FixedUpdate() {
		if (prioritySyncCooldown > 0) {
			prioritySyncCooldown--;
		} else if (slidersDirty) {
			syncPriorityValues();
			prioritySyncCooldown = (int)(0.5 / Time.fixedDeltaTime);
			slidersDirty = false;
		}
	}

	private void syncPriorityValues() {
		float[] priorities = new float[sliders.Count];
		for (int i = 0; i < sliders.Count; i++) {
			priorities [i] = sliders [i].value;
		}

		byte commandByte = PlanningServerGameController.MESSAGE_CLIENT_GAME_PRIORITY;
		byte[] payload = Utils.ObjectToBytes (priorities);

		networkManager.SendData(Utils.ConcatBytes(commandByte, payload));
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		switch (rcvBuffer[0]) {
		case PlanningServerGameController.MESSAGE_SERVER_GAME_STATE:
			byte[] bytes = Utils.SubArray (rcvBuffer, 1, datasize);
			GameState gs = (GameState) Utils.BytesToObject (bytes);
			networkManager.gameState = gs;
			break;
		case PlanningServerGameController.MESSAGE_SERVER_CLIENT_DATA:
			byte[] bytes2 = Utils.SubArray (rcvBuffer, 1, datasize);
			ClientData cd = (ClientData) Utils.BytesToObject (bytes2);
			networkManager.clientData = cd;
			break;
		case PlanningServerGameController.MESSAGE_SERVER_PLAYER_ID:
			this.playerId = (int)rcvBuffer [1];
			networkManager.clientData.playerId = playerId;
			Debug.Log ("Received player id: " + playerId);
			InitializeUI ();
			break;
		case PlanningServerGameController.MESSAGE_SERVER_NEXT_PHASE_SIMULATION:
			SceneManager.LoadScene ("C_simulation");
			break;
		default:
			Debug.LogError ("Received unkown command!");
			break;
		}			
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
