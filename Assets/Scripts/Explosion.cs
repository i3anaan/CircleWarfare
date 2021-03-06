﻿using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public GameObject debrisPrefab;
	public int debrisCount;
	public float force;
	public Color color;

	public void Start() {
		this.transform.SetParent(SimulationServerGameController.GetDebrisStore ().transform);
		debrisPrefab.GetComponents<SpriteRenderer> () [0].color = color;
		for (int i = 0; i < debrisCount; i++) {
			Quaternion rotation = Quaternion.AngleAxis (Random.Range (0f, 360f), Vector3.forward);
			GameObject debris = (GameObject) GameObject.Instantiate (debrisPrefab, this.transform.position, rotation);
			debris.transform.SetParent (SimulationServerGameController.GetDebrisStore ().transform);
			debris.GetComponent<Rigidbody2D>().AddForce(debris.transform.up * force);
		}
	}
}
