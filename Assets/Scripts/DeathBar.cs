using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeathBar : MonoBehaviour {

	public static DeathBar control;

	[Header("//GAMEPLAY SHIT")]
	public float timeLossIncrement;
	public float maxTime;
	public float escapePauseTime;
	public UnityEvent OnDeath;
	public Scarecrow scarecrow;
	[Header("//UI SHIT")]
	public CanvasGroup canvasGroup;
	public float fadeTime;
	public Image[] fillBars;
	public TextMeshProUGUI text;
	[Header("//SHAKE SHIT")]
	public RangeFloat shakeOffset;
	public AnimationCurve shakeCurve;
	[Header("//AUDIO SHIT")]
	public float accel;
	public float audioIntensity;
	public AudioSource ambienceSource;
	public RangeFloat ambienceVolRange;
	public AudioSource droneSource;
	public AudioSource chaseSource;
	public RangeFloat droneVolRange;
	public RangeFloat chaseVolRange;
	[Range(0, 1)]
	public float percentToMax;
	[Space]
	public RandomCrowCaws crowCaws;

	private ScreenShake screenShake;
	private RectTransform rt;
	private Image bg;
	private float currentTime;
	private int numResets;
	private bool isProgressing;
	private float velRef;
	private Coroutine pauseCoroutine;

	private void Awake() {
		control = this;
		screenShake = GetComponent<ScreenShake>();
		rt = GetComponent<RectTransform>();
		bg = GetComponent<Image>();

		currentTime = maxTime;
	}

	private void Update() {
		float ohFuckPercent = 0;
		if (isProgressing) {
			currentTime -= Time.deltaTime;
			ohFuckPercent = currentTime / (maxTime - numResets * timeLossIncrement);
			foreach (Image f in fillBars)
				f.fillAmount = ohFuckPercent;
			if (currentTime <= 0 && !ScreenTransition.isTransitioning) {
				OnDeath.Invoke();
				isProgressing = false;
			}
			screenShake.constantIntensity = shakeOffset.GetAt(shakeCurve.Evaluate(1 - currentTime / maxTime));
		}
		rt.anchoredPosition = screenShake.Output;

		// audio shit

		crowCaws.cawFrequency.SetValueToPercent(1 - ohFuckPercent);

		audioIntensity = Mathf.SmoothDamp(audioIntensity, 1 - (currentTime / maxTime), ref velRef, accel);

		float scaledIntensity = Mathf.Clamp01(audioIntensity / percentToMax);

		if (!WorldControl.gameOver) {
			ambienceSource.volume = ambienceVolRange.GetAt(Extensions.ExpLerp01(scaledIntensity, 2));
			droneSource.volume = droneVolRange.GetAt(Extensions.ExpLerp01(scaledIntensity, 2));
			chaseSource.volume = chaseVolRange.GetAt(Extensions.ExpLerp01(scaledIntensity, 2));
		}
	}

	public void NewRoom(bool scarecrowRoom) {
		if (isProgressing && scarecrowRoom)
			ResetBar();
		else if (isProgressing && !scarecrowRoom)
			StopBar();
		else if (!isProgressing && scarecrowRoom)
			StartBar();
	}

	public void StartBar() {
		currentTime = maxTime;
		numResets = 0;
		foreach (Image f in fillBars)
			f.fillAmount = 1;
		text.enabled = false;
		bg.enabled = false;

		Scarecrow.control.ShowScarecrow(() => {
			StartCoroutine(TransOpacity(1, fadeTime, () => { isProgressing = bg.enabled = text.enabled = true; }));
			EnvControl.control.isAnxious = true;
			if (pauseCoroutine != null) {
				StopCoroutine(pauseCoroutine);
				pauseCoroutine = null;
				Timer.escapePause = false;
			}
			crowCaws.play = true;
		});
	}

	public void ResetBar() {
		numResets++;
		currentTime = maxTime - numResets * timeLossIncrement;
		scarecrow.AppearBehindPlayer();
	}

	public void StopBar() {
		EnvControl.control.isAnxious = false;
		isProgressing = false;
		crowCaws.play = false;
		currentTime = maxTime;
		scarecrow.HideScarecrow();

		StartCoroutine(TransOpacity(0, 0));
		pauseCoroutine = StartCoroutine(EscapePause());
	}

	private IEnumerator TransOpacity(float target, float duration, Action OnComplete = null) {
		float start = canvasGroup.alpha;

		for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime) {
			canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / duration);
			yield return null;
		}
		canvasGroup.alpha = target;

		OnComplete?.Invoke();
	}

	private IEnumerator EscapePause() {
		Timer.escapePause = true;
		for (float elapsed = 0; elapsed < escapePauseTime; elapsed += Time.deltaTime) {

			yield return null;
		}
		Timer.escapePause = false;
		pauseCoroutine = null;
	}
}