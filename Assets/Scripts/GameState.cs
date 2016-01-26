using System;

[Serializable]
public class GameState
{
	public int teams = 0;
	public string[] names = new string[16];

	public override string ToString() {
		return "GameState[teams=" + teams + "]";
	}
}

