using System;
using System.Collections;
using UnityEngine;

public class Scarecrow : MonoBehaviour {

	public static Scarecrow control;
	public static bool isBeingScary;

	[Header("//SETTINGS SHIT")]
	public float lookAtTime;
	public AnimationCurve lookAtCurve;
	public float stunTime;

	private SpriteRenderer primaryRenderer;
	private SpriteRenderer secondaryRenderer;
	private Collider2D primaryCollider;
	private Collider2D secondaryCollider;
	private AudioSource audioSource;
	private float yOffset;

	private void Awake() {
		control = this;
		audioSource = GetComponent<AudioSource>();
		primaryRenderer = GetComponent<SpriteRenderer>();
		secondaryRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		primaryCollider = primaryRenderer.GetComponent<Collider2D>();
		secondaryCollider = secondaryRenderer.GetComponent<Collider2D>();
		yOffset = transform.localPosition.y;
	}

	private void Start() {
		HideScarecrow();
	}

	public void AppearBehindPlayer() {
		transform.localPosition = Vector2.right * EnvControl.control.GetPosition(Mathf.RoundToInt(EnvControl.control.direction + 2) % 4, 2) + Vector2.up * yOffset;
	}

	public void ShowScarecrow(Action OnComplete, Action OnHalfway = null) {
		StartCoroutine(Show(OnComplete, OnHalfway));
	}

	public void HideScarecrow() {
		primaryRenderer.enabled = false;
		secondaryRenderer.enabled = false;
		primaryCollider.enabled = false;
		secondaryCollider.enabled = false;
	}

	private IEnumerator Show(Action OnComplete, Action OnHalfway = null) {
		isBeingScary = true;
		float startDir = EnvControl.control.direction;
		primaryRenderer.enabled = true;
		secondaryRenderer.enabled = true;

		primaryCollider.enabled = true;
		secondaryCollider.enabled = true;

		AppearBehindPlayer();

		// look at
		for (float elapsed = 0; elapsed < lookAtTime; elapsed += Time.deltaTime) {
			while (WorldControl.isPaused)
				yield return null;

			float percent = lookAtCurve.Evaluate(elapsed / lookAtTime);
			EnvControl.control.SetDirection(Mathf.Lerp(startDir, Mathf.RoundToInt(startDir) + 2, percent), true);
			yield return null;
		}
		audioSource.Play();

		for (float elapsed = 0; elapsed < stunTime; elapsed += Time.deltaTime) {
			while (WorldControl.isPaused)
				yield return null;
			yield return null;
		}

		OnComplete.Invoke();
		isBeingScary = false;
	}
}