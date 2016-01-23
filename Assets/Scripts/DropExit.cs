using UnityEngine;
using System.Collections;

public class DropExit : MonoBehaviour {
	public Vector3 direction;
	public float startDropSpeed;
	public float acceleration;
	public bool dropping;
	public float dropDelay;
	private Vector3 speed;
	private int dropDelayTicks;

	void Start() {
		speed = direction.normalized * startDropSpeed;
	}

	void FixedUpdate() {
		if (dropDelayTicks > dropDelay / Time.fixedDeltaTime) {
			StartDrop ();
		}
		if (dropping) {
			this.transform.localPosition = this.transform.localPosition + speed;
			speed += direction.normalized * acceleration;
		}
		dropDelayTicks++;
	}

	public void StartDrop() {
		dropping = true;
	}
}
