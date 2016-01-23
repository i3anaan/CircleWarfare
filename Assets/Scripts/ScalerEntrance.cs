using UnityEngine;
using System.Collections;

public class ScalerEntrance : MonoBehaviour {
	public Vector3 startScale;
	public float scaleTime;
	public float deltaScalePower;
	private int scaleTicks;
	private int currentScaleTick;

	void Start() {
		this.transform.localScale = startScale;
		scaleTicks = (int) (scaleTime / Time.fixedDeltaTime);
	}

	void FixedUpdate() {
		float perc = ((float) currentScaleTick) / scaleTicks;
		float adjPerc = Mathf.Min(Mathf.Pow(perc, deltaScalePower), 1);
		this.transform.localScale = Vector3.Lerp(startScale, new Vector3(1, 1, 1), adjPerc);
		currentScaleTick++;
	}
}
