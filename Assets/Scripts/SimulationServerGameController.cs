﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SimulationServerGameController : BaseServerGameController {

	public static float CREEP_BASE_SPEED = 3f;
	public static float CREEP_BASE_DAMAGE = 1f;
	public static float CREEP_BASE_LIFE = 100f;

	public float speedRandomness;
	public float damageRandomness;
	public float lifeRandomness;

	public Creep creepPrefab;
	public int teamCount;
	public int creepCountPerTeam;
	public List<Creep> creeps;
	public float xRange;
	public float yRange;

	private bool started;

	public static Color[] colors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.grey};
	//TODO move this somewhere else?

	public void StartGame() {
		if (!started) {
			started = true;

			for (int t = 0; t < networkManager.connectionIds.Count; t++) {
				Color color = colors [t];
				int playerId = networkManager.connectionIds [t];
				for (int c = 0; c < creepCountPerTeam; c++) {
					Creep newCreep = (Creep)GameObject.Instantiate (creepPrefab, new Vector3 (), Quaternion.identity);
					ClientData client = networkManager.GetClientData (playerId);
					newCreep.gameObject.GetComponent<SpriteRenderer> ().color = color;
					newCreep.team = t;
					newCreep.color = color;
					newCreep.speed = Random.Range (1f - speedRandomness, 1f + speedRandomness) * CREEP_BASE_SPEED;
					newCreep.damage = Random.Range (1f - damageRandomness, 1f + damageRandomness) * CREEP_BASE_DAMAGE;
					newCreep.life = Random.Range (1f - lifeRandomness, 1f + lifeRandomness) * CREEP_BASE_LIFE;
					newCreep.priorities = client.priorities;

					creeps.Add (newCreep);
				}
			}

			SpawnCreeps ();
		} else {
			Debug.LogError ("Trying to start an already started game!");
		}
	}

	void SpawnCreeps() {
		foreach (Creep creep in creeps) {
			creep.transform.position = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), 0);
		}
	}

	public void GameOver() {
		SceneManager.LoadScene ("S_simulation");
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		StartGame ();
	}
}