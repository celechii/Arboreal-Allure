using UnityEngine;

[CreateAssetMenu(menuName = "Difficulty Config", fileName = "Difficulty _")]
public class DifficultySettings : ScriptableObject {

	public new string name;
	public int minutesToMidnight;
	public float deathPeriod;
	[Range(0, 1)]
	public float targetExitDangerLevel = .5f;
	public bool resetDeathTimerOnEscape = true;

}