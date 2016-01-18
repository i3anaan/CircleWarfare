using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkPlayer : NetworkBehaviour {

	public TeamPriorities priorities;

	[Command]
	public void CmdInformServer() {
		priorities.CmdInformServer ();
	}

	public static NetworkBehaviour getPlayerObject() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) {
			
			if (player.GetComponent<NetworkPlayer>().hasAuthority) {
				return player.GetComponent<NetworkPlayer>();
			}
		}

		return null;
	}
}
