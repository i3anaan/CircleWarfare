using System;
public class ClientData
{
	public int playerId = -1;
	public bool ready = false;
	public string name = "not_set";
	public float[] priorities = new float[16];

	public ClientData(int connectionId) {
		this.playerId = connectionId;
	}

	public override string ToString() {
		return "ClientData[" + name + "]";
	}
}

