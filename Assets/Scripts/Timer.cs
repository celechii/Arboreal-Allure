using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {

	public static bool escapePause;

	public float PercentDeathPeriod {
		get {
			if (deathPeriod == 0)
				return 0;
			return deathPeriodLeft / deathPeriod;
		}
	}
	public float PercentGracePeriod {
		get {
			if (gracePeriod == 0)
				return 0;
			return gracePeriodLeft / gracePeriod;
		}
	}

	public int startTimeHour;
	[Tooltip("the start times at different difficulties")]
	public int[] startTimeMinutes = new int[4];
	public bool midnightGracePeriod;
	public float gracePeriod;
	public float deathPeriod;
	[Space]
	public float timeElapsed;
	[Space]
	public TextMeshProUGUI text;

	private DateTime date;
	private float gracePeriodLeft;
	private float deathPeriodLeft;

	private void Start() {
		StartTimer();
	}

	public void StartTimer() {
		if (Difficulty.config.minutesToMidnight == 0)
			date = new DateTime(2019, 10, 13, 0, 0, 0);
		else
			date = new DateTime(2019, 10, 13, startTimeHour + 12, 60 - Difficulty.config.minutesToMidnight, 0);

		gracePeriod = GetSeconds(gracePeriod);
		deathPeriod = GetSeconds(deathPeriod);

		if (midnightGracePeriod)
			gracePeriod = gracePeriodLeft = (float)(new DateTime(2019, 10, 14, 0, 0, 0) - date).TotalSeconds;
		else
			gracePeriodLeft = gracePeriod;
		deathPeriodLeft = deathPeriod;
		Resume();
	}

	private void Update() {
		if (!WorldControl.isPaused && !escapePause && !GearMachine.isInBridgeArea) {
			timeElapsed += Time.deltaTime;

			if (gracePeriodLeft > 0) {
				gracePeriodLeft -= Time.deltaTime;
				if (gracePeriodLeft < 0) {
					deathPeriodLeft += gracePeriodLeft;
					gracePeriodLeft = 0;
				}
			} else if (deathPeriodLeft > 0) {
				deathPeriodLeft -= Time.deltaTime;
				if (deathPeriodLeft < 0)
					deathPeriodLeft = 0;
			}

			text.text = GetStringTime();
		}
	}

	public void KillGracePeriod() {
		gracePeriodLeft = 0;
	}

	public void Pause() {
		WorldControl.isPaused = true;
	}

	public void Resume() {
		WorldControl.isPaused = false;
	}

	private(int, int)GetMinsAndSeconds(float minutes) => ((int)minutes, Mathf.RoundToInt((minutes % 1) * 60));

	private int GetSeconds(float minutes) {
		(int mins, int seconds) = GetMinsAndSeconds(minutes);
		return mins * 60 + seconds;
	}

	public string GetStringTime() {
		DateTime current = date + new TimeSpan(0, 0, (int)timeElapsed);
		return $"{current.ToString("hh")}<color=#ffffff{(current.Second % 2 == 0 ? "00" : "ff")}>:</color>{current.ToString("mm tt")}";
	}
}