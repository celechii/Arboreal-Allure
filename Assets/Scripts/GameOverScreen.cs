using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

	public static GameOverScreen control;

	[TextArea]
	public string loseText;
	public string losePrompt;
	[TextArea]
	public string winText;
	public string winPrompt;
	[Space]
	public float fadeInTime;
	public CanvasGroup canvasGroup;
	public TextMeshProUGUI endText;
	public TextMeshProUGUI promptText;
	public AudioClip deathSting;
	public AudioClip escapeSting;
	public AudioSource[] audioSourcesToMute;

	private Image bg;
	private AudioSource audioSource;
	private Coroutine resetCoroutine;
	private bool isFading;

	private void Awake() {
		control = this;
		bg = GetComponent<Image>();
		audioSource = GetComponent<AudioSource>();
	}

	private void Update() {
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R))
			ResetGame();
		if (Input.GetKeyDown(KeyCode.Q)) {
			WorldControl.gameOver = true;
			ShowEndScreen(false);
		}
		#endif

		if (!WorldControl.gameOver) {
			if (InputManager.GetAxisDown("Difficulty") && !isFading) {
				Difficulty.Level += InputManager.GetAxis("Difficulty");
				if (resetCoroutine != null)
					StopCoroutine(resetCoroutine);
				resetCoroutine = StartCoroutine(Reset(false, 1f));
			}
		}
	}

	private void Start() {
		StartCoroutine(FadeBG(false, fadeInTime));
	}

	public void ShowEndScreen(bool win) {
		audioSource.PlayOneShot(win?escapeSting : deathSting);

		endText.text = win ? winText : loseText;
		promptText.text = win ? winPrompt : losePrompt;

		bg.raycastTarget = true;
		CursorControl.ClearAll();
		StartCoroutine(ShowScreen(win));
		StartCoroutine(SilenceAmbience());
	}

	public void ResetGame() {
		StartCoroutine(Reset());
	}

	private IEnumerator SilenceAmbience() {
		float[] startingPercents = new float[audioSourcesToMute.Length];
		for (int i = 0; i < startingPercents.Length; i++)
			startingPercents[i] = audioSourcesToMute[i].volume;

		for (float elapsed = 0; elapsed < fadeInTime; elapsed += Time.deltaTime) {
			float percent = 1 - (elapsed / fadeInTime);
			for (int i = 0; i < audioSourcesToMute.Length; i++) {
				audioSourcesToMute[i].volume = Extensions.ExpLerp01(percent * startingPercents[i], 2);
			}
			yield return null;
		}
	}

	private IEnumerator FadeBG(bool on, float time) {
		isFading = true;
		Color col = bg.color;
		for (float elapsed = 0; elapsed < time; elapsed += Time.deltaTime) {
			col.a = elapsed / time;
			if (!on)
				col.a = 1 - col.a;
			bg.color = col;
			yield return null;
		}
		col.a = on ? 1 : 0;
		bg.color = col;
		isFading = false;
	}

	private IEnumerator ShowScreen(bool win) {
		canvasGroup.blocksRaycasts = true;
		if (!win)
			yield return StartCoroutine(FogControl.control.Perish());

		canvasGroup.alpha = 0;
		yield return StartCoroutine(FadeBG(true, fadeInTime / 2f));

		for (float elapsed = 0; elapsed < fadeInTime; elapsed += Time.deltaTime) {
			canvasGroup.alpha = elapsed / fadeInTime;
			yield return null;
		}
		canvasGroup.alpha = 1;
	}

	private IEnumerator Reset(bool fadeCanvasGroup = true, float delay = 0) {

		yield return new WaitForSeconds(delay);

		canvasGroup.interactable = false;
		if (fadeCanvasGroup) {
			for (float elapsed = 0; elapsed < fadeInTime; elapsed += Time.deltaTime) {
				canvasGroup.alpha = 1 - (elapsed / fadeInTime);
				yield return null;
			}
		} else
			yield return StartCoroutine(FadeBG(true, .5f));

		resetCoroutine = null;

		canvasGroup.alpha = 0;
		WorldControl.gameOver = false;
		Gear.hasGear = false;
		SceneManager.LoadScene(0);
	}
}