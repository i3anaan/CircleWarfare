using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClientNetworkManager : CustomNetworkManager {

	void Awake() {
		ConnectAsClient (7523, "0.0.0.0", 7522);
	}

	void FixedUpdate() {
		SendData (new byte[]{ 1 }, 1);		
	}
}
