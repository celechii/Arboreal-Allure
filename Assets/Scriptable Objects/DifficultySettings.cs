using UnityEngine;

[CreateAssetMenu(menuName = "Difficulty Config", fileName = "Difficulty _")]
public class DifficultySettings : ScriptableObject {

	public new string name;
	public int minutesToMidnight;
	public float deathPeriod;

}