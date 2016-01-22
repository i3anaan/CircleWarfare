using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlanningClientGameController : BaseClientGameController {

	public Slider sliderPrefab;
	public Canvas canvas;

	private List<Slider> sliders;
	private bool slidersDirty;
	private int prioritySyncCooldown;

	private GameState gameState;
	public int playerId;

	public void Awake() {
		base.Awake ();
	}

	private void InitializeUI() {
		sliders = new List<Slider> ();
		//int sliderCount = networkManager.gameState.teams;
		int sliderCount = 5;
		for (int i = 0; i < sliderCount; i++) {
			Vector3 sliderPos = new Vector3 (-(sliderCount / 2) * 100 + i * 100, 0, 0);
			Slider slider = (Slider) GameObject.Instantiate (sliderPrefab, sliderPos, Quaternion.identity);
			sliders.Add (slider);
			slider.onValueChanged.AddListener (delegate {SliderChanged ();});
			slider.transform.SetParent (canvas.transform, false);
			slider.transform.localScale = new Vector3 (1, 1, 1);
			//slider.transform.position = sliderPos;
			if (i == playerId) {
				slider.enabled = false;
			}
		}
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
			Debug.Log ("Received GameState: " + gs);
			break;
		case PlanningServerGameController.MESSAGE_SERVER_PLAYER_ID:
			this.playerId = (int)rcvBuffer [1];
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
}
