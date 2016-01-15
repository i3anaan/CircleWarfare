using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creep : MonoBehaviour {

	public static float BASE_SPEED = 0.03f;
	public static float BASE_DAMAGE = 1f;
	public static float BASE_LIFE = 100f;

	private GameController gc;
	private List<Creep> creeps;
	public int team;
	public float speed;
	public float damage;
	public float life;
	private float[] priority = new float[]{1f, 1f, 1f, 3f};

	// Use this for initialization
	void Start () {
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		creeps = gc.creeps;
		ReScale ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Creep target = FindTarget ();

		//Vector3 dir = (target.transform.position - this.transform.position).normalized;

		this.transform.position = Vector3.MoveTowards (this.transform.position, target.transform.position, speed);

		if (Vector3.Distance (this.transform.position, target.transform.position) < (this.transform.localScale.x + target.transform.localScale.x) * 0.6f) {
			//Fighting the target;
			this.Attack (target);
		}
	}

	void TakeDamage(float damage) {
		life -= damage;
		ReScale ();

		if (this.life < 0) {
			GameObject.Destroy (this.gameObject);
		}
	}

	void Attack(Creep target) {
		target.TakeDamage(this.damage);
	}

	void ReScale() {
		float scale = this.life / BASE_LIFE;
		this.transform.localScale = new Vector3 (scale, scale, scale);
	}

	Creep FindTarget() {
		float minDistance = float.MaxValue;
		Creep target = null;

		foreach (Creep c in creeps) {
			if (c && c.team != this.team) {
				float dist = Vector3.Distance(this.transform.position, c.gameObject.transform.position);
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
