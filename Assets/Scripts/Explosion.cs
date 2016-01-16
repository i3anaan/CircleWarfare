using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Explosion : NetworkBehaviour {

	public GameObject debrisPrefab;
	public int debrisCount;
	public float force;
	public Color color;

	void Start() {
		debrisPrefab.GetComponents<SpriteRenderer> () [0].color = color;
		for (int i = 0; i < debrisCount; i++) {
			Quaternion rotation = Quaternion.AngleAxis (Random.Range (0f, 360f), Vector3.forward);
			GameObject debris = (GameObject) GameObject.Instantiate (debrisPrefab, this.transform.position, rotation);
			debris.GetComponent<Rigidbody2D>().AddForce(debris.transform.up * force);
			NetworkServer.Spawn(debris.gameObject);
		}
	}
}
