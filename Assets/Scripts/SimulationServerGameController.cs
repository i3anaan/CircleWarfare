using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SimulationServerGameController : BaseServerGameController {

	public const byte MESSAGE_SERVER_GAME_OVER = 9;
	public const byte MESSAGE_SERVER_NEXT_PHASE_PLANNING = 10;

	public static float CREEP_BASE_SPEED = 2.5f;
	public static float CREEP_BASE_DAMAGE = 0.8f;
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
	public bool gameOver;
	public bool firstBlood;
	private int teamsStillAlive;

	public TextSplasher splasher;
	public GameObject firstBloodSplash;

	private static GameObject creepStore;
	private static GameObject debrisStore;
	private static float TIME_SCALE = 1;
	private static float TIME_SCALE_PREVIOUS = 1;
	private float timeScaleSlow;
	private int timeScaleSlowTicks;
	private int currentTimeScaleSlowTicks;

	public static Color[] colors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.grey};
	public static string[] colorNames = new string[]{"RED", "BLUE", "GREEN", "YELLOW", "CYAN", "MAGENTA", "GREY"};
	//TODO move this somewhere else?

	public static GameObject GetCreepStore() {
		if (creepStore == null) {
			creepStore = new GameObject ("creepStore");
		}
		return creepStore;
	}
	public static GameObject GetDebrisStore() {
		if (debrisStore == null) {
			debrisStore = new GameObject ("debrisStore");
		}
		return debrisStore;
	}

	public static float GetSimulationTimeScale() {
		return TIME_SCALE;
	}

	public void Start() {
		SetSimulationTimeScale (1);
		StartGame ();
	}

	public void FixedUpdate() {
		//Recover from a slow down effect.
		if (currentTimeScaleSlowTicks >= 0) {
			currentTimeScaleSlowTicks++;
			float perc = ((float) currentTimeScaleSlowTicks) / timeScaleSlowTicks;
			float percAdj = Mathf.Pow (perc, 5);

			if (currentTimeScaleSlowTicks < timeScaleSlowTicks) {
				SetSimulationTimeScale (1 - (1 - timeScaleSlow) * (1-percAdj));
			} else {
				SetSimulationTimeScale (1);
				currentTimeScaleSlowTicks = -1;
			}				
		}
	}

	public void StartGame() {
		if (!started) {
			started = true;
			teamsStillAlive = networkManager.gameState.teams;
			SpawnCreeps ();
		} else {
			Debug.LogWarning ("Trying to start an already started game!");
		}
	}

	public void SpawnCreeps() {
		for (int t = 0; t < networkManager.gameState.teams; t++) {
		//for (int t = 0; t < 4; t++) {
			Color color = colors [t];
			int playerId = networkManager.connectionIds [t];

			ClientData client = networkManager.GetClientData (playerId);
			float handicap = ((float) client.handicapPoints) / (3 * networkManager.gameState.teams) + 1;
			handicap = Mathf.Pow(handicap, 2);

			for (int c = 0; c < creepCountPerTeam; c++) {
				Vector3 pos = new Vector3 (Random.Range (-xRange, xRange), Random.Range (-yRange, yRange), 0);
				Creep newCreep = (Creep)GameObject.Instantiate (creepPrefab, pos, Quaternion.identity);
				newCreep.transform.SetParent (GetCreepStore().transform);
				newCreep.gc = this;
				newCreep.creeps = creeps;
				newCreep.TimeScale = TIME_SCALE;
				newCreep.gameObject.GetComponent<SpriteRenderer> ().color = color;
				newCreep.team = t;
				newCreep.color = color;
				newCreep.speed = Random.Range (1f - speedRandomness, 1f + speedRandomness) * CREEP_BASE_SPEED * handicap;
				newCreep.damage = Random.Range (1f - damageRandomness, 1f + damageRandomness) * CREEP_BASE_DAMAGE * handicap;
				newCreep.life = Random.Range (1f - lifeRandomness, 1f + lifeRandomness) * CREEP_BASE_LIFE * handicap;
				newCreep.priorities = client.priorities;

				creeps.Add (newCreep);
			}
		}
	}

	public void SlowSimulation(float slow, float duration) {
		SetSimulationTimeScale (slow);
		timeScaleSlow = slow;
		timeScaleSlowTicks = (int) (duration / Time.fixedDeltaTime);
		currentTimeScaleSlowTicks = 0;
	}

	public void SetSimulationTimeScale(float timeScale) {
		TIME_SCALE_PREVIOUS = TIME_SCALE;
		TIME_SCALE = timeScale;
		foreach (Creep c in creeps) {
			if (c != null) {
				c.TimeScale = timeScale;
			}
		}

		foreach (Rigidbody2D body in GetDebrisStore().transform.GetComponentsInChildren<Rigidbody2D>()) {
			//body.mass = Explosion.DEBRIS_BASE_MASS / TIME_SCALE;
			//While i believe setting the mass according the time scale would be correct, not doing so looks better :)
			Vector2 newRealVel = body.velocity * (TIME_SCALE / TIME_SCALE_PREVIOUS);
			body.velocity = newRealVel;
		}
	}

	public void GameOver(int winningTeam) {
		if (!gameOver) {
			ClientData client = networkManager.GetClientData (winningTeam + 1);
			client.wins++;
			client.handicapPoints = Mathf.Max (client.handicapPoints - 5, 0);
			gameOver = true;
			SendGameOverData (winningTeam + 1);
			splasher.Splash ("VICTORY!\n" + client.name.ToUpper(), 10, 1);
			Invoke ("NextScene", 9);
		}
	}

	private void NextScene() {
		SendNextSceneData ();
		SceneManager.LoadScene ("S_planning");
	}

	public void TeamLost(int team) {
		GetClient (team + 1).handicapPoints += teamsStillAlive;
		teamsStillAlive--;
	}

	private void SendGameOverData(int winnerId) {
		byte command = MESSAGE_SERVER_GAME_OVER;
		byte winner = (byte) winnerId;
		networkManager.SendDataAll (Utils.ConcatBytes (command, winner));
	}

	private void SendNextSceneData() {
		byte command = MESSAGE_SERVER_NEXT_PHASE_PLANNING;
		networkManager.SendDataAll (command);
	}

	public override void RcvData(int rcvHostId, int connectionId, int channelId, byte[] rcvBuffer, int datasize) {
		StartGame ();
	}

	public void FirstBlood() {
		firstBlood = true;
		SlowSimulation (0.1f, 3f);
		splasher.SplashPrefab (firstBloodSplash);
	}
}
