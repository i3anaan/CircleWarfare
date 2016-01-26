using System;
[Serializable]
public class ClientData
{
	public int playerId = -1;
	public bool ready = false;
	public string name = "not_set";
	public float[] priorities = new float[16];
	public int wins = 0;
	public int handicapPoints = 0;

	public ClientData() : this(-1) {
	}

	public ClientData(int connectionId) {
		this.playerId = connectionId;

		for (int i = 0; i < priorities.Length; i++) {
			priorities[i] = 1;
		}
	}

	public override string ToString() {
		return "ClientData[" + name + "]";
	}
}

