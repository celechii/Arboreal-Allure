using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour {

	private static ScreenTransition control;
	public static bool isTransitioning;

	public float duration;
	public AnimationCurve fadeCurve;

	private Image bg;

	private void Awake() {
		control = this;
		bg = GetComponent<Image>();
	}

	public static void Transition(float duration = .1f, Action OnHalfway = null, Action OnComplete = null) {
		control.StartCoroutine(control.DoTransition(duration, OnHalfway, OnComplete));
	}

	private IEnumerator DoTransition(float duration = .1f, Action OnHalfway = null, Action OnComplete = null) {
		isTransitioning = true;
		Color colour = Color.clear;

		for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime) {
			colour.a = fadeCurve.Evaluate(elapsed / duration);
			bg.color = colour;
			yield return null;
		}

		OnHalfway?.Invoke();

		for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime) {
			colour.a = fadeCurve.Evaluate(1 - (elapsed / duration));
			bg.color = colour;
			yield return null;
		}
		bg.color = Color.clear;

		isTransitioning = false;
		OnComplete?.Invoke();
	}

}