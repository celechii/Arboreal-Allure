using UnityEngine;
using UnityEngine.Events;

public class Gear : MonoBehaviour {

	public static bool hasGear;

	public Sprite defaultSprite;
	public Sprite highlightSprite;
	public Sprite takenSprite;
	public UnityEvent OnTake;

	private AudioSource audioSource;
	private SpriteRenderer spriteRenderer;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnMouseOver() {
		if (hasGear)
			return;
		CursorControl.Interaction = true;
		if (!hasGear)
			spriteRenderer.sprite = highlightSprite;
	}

	private void OnMouseExit() {
		if (hasGear)
			return;
		CursorControl.Interaction = false;
		if (!hasGear)
			spriteRenderer.sprite = defaultSprite;
	}

	private void OnMouseUpAsButton() {
		if (hasGear && CursorControl.CurrentState != CursorControl.CursorState.Interaction)
			return;
		hasGear = true;
		audioSource.Play();
		OnTake.Invoke();
		Gear[] gears = FindObjectsOfType<Gear>();
		foreach (Gear g in gears)
			g.TakeGear();
		CursorControl.Interaction = false;
	}

	public void TakeGear() {
		spriteRenderer.sprite = takenSprite;
	}
}