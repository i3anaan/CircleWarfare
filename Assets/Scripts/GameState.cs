using System;

[Serializable]
public class GameState
{
	public int teams;


	public override string ToString() {
		return "GameState[teams=" + teams + "]";
	}
}

