using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveDirections : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public NFloatEvent onHover;
	[SerializeField]
	private Dir dir;
	public AnimationCurve moveCurve;

	private RectTransform rt;
	private Button button;
	private bool isHovering;

	private void Awake() {
		rt = GetComponent<RectTransform>();
		button = GetComponent<Button>();
	}

	private void Update() {
		if (isHovering && CursorControl.CurrentState == CursorControl.CursorState.Turning) {
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out pos);
			float value = 1 - Mathf.Abs(pos.x) / rt.sizeDelta.x;
			onHover.Invoke(moveCurve.Evaluate(value) * (dir == Dir.Left? - 1 : 1));
		}
	}

	public void Enable(bool enable) {
		button.interactable = enable;
		OnPointerExit(null);
	}

	public static void EnableMoveBars(bool enable) {
		foreach (MoveDirections m in GameObject.FindObjectsOfType<MoveDirections>())
			m.Enable(enable);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		isHovering = true;
		if (dir == Dir.Left)
			CursorControl.TurnLeft = true;
		else
			CursorControl.TurnRight = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		isHovering = false;
		if (dir == Dir.Left)
			CursorControl.TurnLeft = false;
		else
			CursorControl.TurnRight = false;
	}

	private enum Dir {
		Left,
		Right
	}
}