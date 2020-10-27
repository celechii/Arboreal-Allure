using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FogControl : MonoBehaviour {

	public static FogControl control;

	public Gradient colourGradient;
	public Color endGraceColour;
	public Color endDeathColour;
	public Color perishColour;
	public Timer timer;
	public float perishTime;
	public float colourAccel;

	private Image[] images;
	private bool isAlive;
	private Color markedColour;
	private Color targetColour;
	private Color currentColour = Color.clear;
	private Vector4 velRef;

	private void Awake() {
		control = this;
		images = GetComponentsInChildren<Image>();
		isAlive = true;
		markedColour = endGraceColour;
	}

	private void Update() {

		if (isAlive) {
			if (timer.PercentGracePeriod == 0) {
				targetColour = Color.Lerp(markedColour, endDeathColour, MapControl.control.GetScarecrowZoneLevel());
				// UpdateColour(Color.Lerp(markedColour, endDeathColour, MapControl.control.GetScarecrowZoneLevel()));
			} else {
				targetColour = Color.Lerp(Color.clear, endGraceColour, 1 - timer.PercentGracePeriod);
				// UpdateColour(Color.Lerp(Color.clear, endGraceColour, 1 - timer.PercentGracePeriod));
			}

			currentColour = Extensions.ColourSmoothDamp(currentColour, targetColour, ref velRef, colourAccel);
			UpdateColour(currentColour);
		}
	}

	private void OnValidate() {
		UpdateColour(colourGradient.Evaluate(0));
	}

	public void UpdateColour(Color color) {
		if (images == null)
			images = GetComponentsInChildren<Image>();

		foreach (Image i in images)
			i.color = color;
	}

	public void MarkCurrentColour() {
		markedColour = images[0].color;
	}

	public IEnumerator Perish() {
		Color startColour = images[0].color;
		for (float elapsed = 0; elapsed < perishTime; elapsed += Time.deltaTime) {
			UpdateColour(Color.Lerp(startColour, perishColour, elapsed / perishTime));
			yield return null;
		}
		UpdateColour(perishColour);
		isAlive = false;
	}
}