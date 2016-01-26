using System;

[Serializable]
public class GameState
{
	public int teams = 0;

	public override string ToString() {
		return "GameState[teams=" + teams + "]";
	}
}

