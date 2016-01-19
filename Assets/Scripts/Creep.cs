using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creep : MonoBehaviour {

	private GameController gc;
	private List<Creep> creeps;
	public int team;
	public float speed;
	public float damage;
	public float life;
	public Explosion explosionPrefab;
	public Color color;
	private float[] priority = new float[]{1f, 1f, 1f, 1f};

	// Use this for initialization
	void Start () {
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		creeps = gc.creeps;
		for (int i = 0; i < priority.Length; i++) {
			priority [i] = Random.Range (1f, 3f);
		}
		ReScale ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Creep target = FindTarget ();

		if (target) {
			Vector3 dir = (target.transform.position - this.transform.position).normalized;
			float mass = this.GetComponent<Rigidbody2D> ().mass;
			this.GetComponents<Rigidbody2D> () [0].AddForce (dir * speed * mass);

			if (Vector2.Distance (this.transform.position, target.transform.position) < (this.transform.localScale.x + target.transform.localScale.x) * 0.51f) {
				//Fighting the target;
				this.Attack (target);
			}
		}
	}

	void TakeDamage(float damage) {
		life -= damage;
		ReScale ();

		if (this.life < 0) {
			Die ();
		}
	}

	void Die() {
		//Explode
		/*
		float explDist = 3f;
		float explForce = 100f;

		foreach (Creep c in creeps) {
			if (c) {
				float dist = Vector2.Distance(this.transform.position, c.transform.position);
				dist = dist - c.transform.localScale.x;
				if (dist < explDist) {
					Vector3 dir = (c.transform.position - this.transform.position).normalized;
					Vector3 force = dir * (dist / explDist) * explForce;
					c.GetComponent<Rigidbody2D>().AddForce(force);
				}
			}
		}
		*/
		Explosion exp = (Explosion) GameObject.Instantiate (explosionPrefab, this.transform.position, this.transform.rotation);
		exp.color = this.color;
		GameObject.Destroy (this.gameObject);
	}

	void Attack(Creep target) {
		target.TakeDamage(this.damage);
	}

	void ReScale() {
		float scale = Mathf.Max(this.life / GameController.CREEP_BASE_LIFE, 0.15f);
		this.transform.localScale = new Vector3 (scale, scale, scale);
		this.GetComponent<Rigidbody2D> ().mass = 0.5f + scale / 2f;
	}

	Creep FindTarget() {
		float minDistance = float.MaxValue;
		Creep target = null;

		foreach (Creep c in creeps) {
			if (c && c.team != this.team) {
				float dist = Vector2.Distance(this.transform.position, c.gameObject.transform.position);
				if (dist / priority[c.team] < minDistance) {
					minDistance = dist / priority[c.team];
					target = c;
				}
			}
		}

		if (!target) {
			gc.GameOver();
		}

		return target;
	}
}
