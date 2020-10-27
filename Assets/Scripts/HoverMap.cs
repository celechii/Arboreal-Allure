using UnityEngine;
using UnityEngine.EventSystems;

public class HoverMap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	public void OnPointerEnter(PointerEventData eventData) {
		CursorControl.DrawMapHover = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		CursorControl.DrawMapHover = false;
	}

	public void OnPointerDown(PointerEventData eventData) {
		CursorControl.DrawMapDown = true;
	}

	public void OnPointerUp(PointerEventData eventData) {
		CursorControl.DrawMapDown = false;
	}
}