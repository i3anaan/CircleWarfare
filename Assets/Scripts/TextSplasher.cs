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
		public GameObject obj;
	}

	void Awake() {
		queue = new List<SplashCommand>();
	}

	void FixedUpdate() {
		if (queue.Count > 0) {
			SplashCommand cmd = queue [0];
			queue.RemoveAt (0);
			if (cmd.obj == null) {
				Splash (cmd.text, cmd.duration);
			} else {
				Splash (cmd.obj);
			}
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

	public void SplashPrefab(GameObject splash) {
		SplashCommand cmd = new SplashCommand();
		cmd.obj = splash;

		queue.Add(cmd);
	}
		

	private void Splash(string text, float duration) {
		splash = Instantiate (splashPrefab);
		splash.transform.SetParent (canvas.transform, false);
		splash.transform.GetChild (0).GetComponent<Text> ().text = text;
		splash.GetComponent<DropExit> ().dropDelay = duration;
	}

	private void Splash(GameObject customPrefab) {
		splash = Instantiate (customPrefab);
		splash.transform.SetParent (canvas.transform, false);
	}		
}
