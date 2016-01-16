using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TeamPriorities : NetworkBehaviour {

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void CmdInformServer() {
		Debug.Log("Server: InformServer()");
	}
}
