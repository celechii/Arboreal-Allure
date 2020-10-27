using System.Collections;
using UnityEngine;

public class OpeningUI : MonoBehaviour {

	public string buttonName;
	public int showHeight;
	public int hideHeight;
	public AudioClip openSFX;
	public AudioClip closeSFX;
	[Space]
	public AnimationCurve openCurve;
	public float openDuration;
	public float closeDuration;

	private AudioSource audioSource;
	private RectTransform rt;
	private bool shouldBeOpen;
	private bool isOpen;
	private Coroutine coroutine;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
		rt = GetComponent<RectTransform>();
	}

	private void Update() {
		if (InputManager.GetKeyDown(buttonName) && !WorldControl.gameOver) {
			shouldBeOpen = true;
			if (coroutine == null)
				coroutine = StartCoroutine(Toggle(shouldBeOpen));
		}

		if (InputManager.GetKeyUp(buttonName) || WorldControl.gameOver && isOpen) {
			shouldBeOpen = false;
			if (coroutine == null)
				coroutine = StartCoroutine(Toggle(shouldBeOpen));
		}
	}

	private IEnumerator Toggle(bool open) {
		audioSource.PlayOneShot(open?openSFX : closeSFX);
		float duration = open ? openDuration : closeDuration;
		for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime) {
			float percent = elapsed / duration;
			if (!open)
				percent = 1 - percent;
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.LerpUnclamped(hideHeight, showHeight, openCurve.Evaluate(percent)));
			yield return null;
		}
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, open ? showHeight : hideHeight);

		isOpen = open;
		if (shouldBeOpen != isOpen)
			coroutine = StartCoroutine(Toggle(shouldBeOpen));
		else
			coroutine = null;
	}
}