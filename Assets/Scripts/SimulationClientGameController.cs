using UnityEngine;
using System.Collections;

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
}
