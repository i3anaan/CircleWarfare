using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TextSplasher : MonoBehaviour {
	public Canvas canvas;
	public GameObject splashPrefab;
	private GameObject splash;
	private List<SplashCommand> queue;

	private struct SplashCommand
	{
		public string text;
		public float duration;
	}

	void Awake() {
		queue = new List<SplashCommand>();
		Splash ("TRIPLE KILL", 3, 10);
	}

	void FixedUpdate() {
		if (queue.Count > 0) {
			SplashCommand cmd = queue [0];
			queue.RemoveAt (0);
			Splash (cmd.text, cmd.duration);
		}
	}

	public void Splash (string text, float duration, int amount) {
		SplashCommand cmd = new SplashCommand();
		cmd.text = text;
		cmd.duration = duration;

		for (int i = 0; i < amount; i++) {
			queue.Add(cmd);
		}
	}

	private void Splash(string text, float duration) {
		splash = Instantiate (splashPrefab);
		splash.transform.SetParent (canvas.transform, false);
		splash.transform.GetChild (0).GetComponent<Text> ().text = text;
		splash.GetComponent<DropExit> ().dropDelay = duration;
	}
		
}
