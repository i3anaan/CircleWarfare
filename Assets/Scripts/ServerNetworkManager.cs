using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerNetworkManager : CustomNetworkManager {
	void Start () {
		SetupAsServer (7522, 4);
	}
}
