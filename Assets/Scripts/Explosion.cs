using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public static float DEBRIS_BASE_MASS = 0.07f;
	public GameObject debrisPrefab;
	public int debrisCount;
	public static float DEBRIS_FORCE = 15;
	public Color color;

	public void Start() {
		float timeScale = SimulationServerGameController.GetSimulationTimeScale ();
		float specialTimeScale = (timeScale - 1) * 0.3f + 1;

		this.transform.SetParent(SimulationServerGameController.GetDebrisStore ().transform);
		debrisPrefab.GetComponents<SpriteRenderer> () [0].color = color;
		for (int i = 0; i < debrisCount; i++) {
			Quaternion rotation = Quaternion.AngleAxis (Random.Range (0f, 360f), Vector3.forward);
			GameObject debris = (GameObject) GameObject.Instantiate (debrisPrefab, this.transform.position, rotation);
			debris.transform.SetParent (SimulationServerGameController.GetDebrisStore ().transform);
			debris.GetComponent<Rigidbody2D> ().mass = DEBRIS_BASE_MASS / timeScale;
			debris.GetComponent<Rigidbody2D>().AddForce(debris.transform.up * DEBRIS_FORCE);
		}
	}
}
