using System;
public class ClientData
{
	public int connectionId = -1;
	public bool ready = false;
	public string name = "not_set";

	public ClientData(int connectionId) {
		this.connectionId = connectionId;
	}

	public override string ToString() {
		return "ClientData[" + name + "]";
	}
}

