using UnityEngine;

public class WorldControl : MonoBehaviour {

	public static bool isPaused;
	public static bool gameOver;

	public void GameOver() {
		gameOver = true;
	}
}