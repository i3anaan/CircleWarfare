using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class GameController : NetworkBehaviour {

	public Creep creepPrefab;
	public int teamCount;
	public int creepCountPerTeam;
	public List<Creep> creeps;
	public float xRange;
	public float yRange;

	private bool started;

	private Color[] colors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow};

	void Start() {
		NetworkServer.Spawn (this.gameObject);
	}


	[Server]
	void StartGame() {
		started = true;
		Debug.Log ("GameController.Awake();");

		for (int t = 0; t < teamCount; t++) {
			Color color = colors [t];
			for (int c = 0; c < creepCountPerTeam; c++) {
				Creep newCreep = (Creep) GameObject.Instantiate (creepPrefab, new Vector3 (), Quaternion.identity);
				NetworkServer.Spawn (newCreep.gameObject);
				newCreep.gameObject.GetComponent<SpriteRenderer>().color = color;
				newCreep.team = t;
				newCreep.color = color;
				newCreep.speed = Random.Range (0.5f, 1.5f) * Creep.BASE_SPEED;
				newCreep.damage = Random.Range (0.5f, 1.5f) * Creep.BASE_DAMAGE;
				newCreep.life = Random.Range(0.5f, 1.5f) * Creep.BASE_LIFE;
				creeps.Add (newCreep);
			}
		}

		SpawnCreeps ();
	}

	[Server]
	void Update() {
		if (!started && Input.GetKeyDown(KeyCode.Space)) {
			StartGame ();
		}
	}


	void SpawnCreeps() {
		foreach (Creep creep in creeps) {
			creep.transform.position = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), 0);
		}
	}

	public void GameOver() {
		Application.LoadLevel ("main");
	}
}
