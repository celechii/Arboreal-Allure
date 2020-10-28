using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DifficultyLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	private Difficulty controller;
	private Image image;
	private int level;

	private void Awake() {
		controller = transform.parent.parent.GetComponent<Difficulty>();
		level = transform.GetSiblingIndex();
		image = GetComponent<Image>();
	}

	public void SetColor(Color colour) {
		image.color = colour;
	}

	public void SetSprite(Sprite sprite) {
		image.sprite = sprite;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		controller.levelHovers[level] = true;
		controller.UpdateViews();
	}

	public void OnPointerExit(PointerEventData eventData) {
		controller.levelHovers[level] = false;
		controller.UpdateViews();
	}

	public void OnPointerClick(PointerEventData eventData) {
		Difficulty.Level = level;
	}
}