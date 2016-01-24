using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creep : MonoBehaviour {

	public SimulationServerGameController gc;
	public List<Creep> creeps;
	public int team;
	public float speed;
	public float damage;
	public float life;
	public Explosion explosionPrefab;
	public Color color;
	public float[] priorities = new float[]{1f, 1f, 1f, 1f};

	private float timeScale;
	public float TimeScale 
	{
		get 
		{ 
			return timeScale; 
		}
		set 
		{
			timeScale = value;
			AdjustToTimeScale ();
		}
	}

	// Use this for initialization
	void Start () {
		ReScale ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Creep target = FindTarget ();

		if (target) {
			Vector3 dir = (target.transform.position - this.transform.position).normalized;
			float mass = this.GetComponent<Rigidbody2D> ().mass;
			this.GetComponents<Rigidbody2D> () [0].AddForce (dir * speed);

			if (Vector2.Distance (this.transform.position, target.transform.position) < (this.transform.localScale.x + target.transform.localScale.x) * 0.55f) {
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
		Explosion exp = (Explosion) GameObject.Instantiate (explosionPrefab, this.transform.position, this.transform.rotation);
		exp.color = this.color;
		if (!gc.firstBlood) {
			gc.FirstBlood ();
		}
		GameObject.Destroy (this.gameObject);
	}

	void Attack(Creep target) {
		target.TakeDamage(this.damage * timeScale);
	}

	void ReScale() {
		float scale = Mathf.Max(this.life / SimulationServerGameController.CREEP_BASE_LIFE, 0.15f);
		this.transform.localScale = new Vector3 (scale, scale, scale);
		this.GetComponent<Rigidbody2D> ().mass = (0.5f + scale / 2f) / timeScale;
	}

	Creep FindTarget() {
		float minDistance = float.MaxValue;
		Creep target = null;

		foreach (Creep c in creeps) {
			if (c && c.team != this.team) {
				float dist = Vector2.Distance(this.transform.position, c.gameObject.transform.position);
				if (dist / priorities[c.team] < minDistance) {
					minDistance = dist / priorities[c.team];
					target = c;
				}
			}
		}

		if (!target) {
			gc.GameOver();
		}
		return target;
	}

	private void AdjustToTimeScale () {
		ReScale ();
	}
}
