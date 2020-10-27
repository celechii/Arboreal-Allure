using UnityEngine;
using UnityEngine.UI;

public class CursorControl : MonoBehaviour {
	private static CursorControl control;

	private static bool turnLeft;
	private static bool turnRight;
	private static bool goForwards;
	private static bool carveWoodHover;
	private static bool carveWoodDown;
	private static bool drawMapHover;
	private static bool drawMapDown;
	private static bool interaction;
	private static bool hoverButton;

	public static bool TurnLeft { get { return turnLeft; } set { turnLeft = value; control.UpdateCursor(); } }
	public static bool TurnRight { get { return turnRight; } set { turnRight = value; control.UpdateCursor(); } }
	public static bool GoForwards { get { return goForwards; } set { goForwards = value; control.UpdateCursor(); } }
	public static bool CarveWoodHover { get { return carveWoodHover; } set { carveWoodHover = value; control.UpdateCursor(); } }
	public static bool CarveWoodDown { get { return carveWoodDown; } set { carveWoodDown = value; control.UpdateCursor(); } }
	public static bool DrawMapHover { get { return drawMapHover; } set { drawMapHover = value; control.UpdateCursor(); } }
	public static bool DrawMapDown { get { return drawMapDown; } set { drawMapDown = value; control.UpdateCursor(); } }
	public static bool Interaction { get { return interaction; } set { interaction = value; control.UpdateCursor(); } }
	public static bool HoverButton { get { return hoverButton; } set { hoverButton = value; control.UpdateCursor(); } }
	public static CursorState CurrentState { get { return control.currentState; } }

	public CursorState currentState;
	[Space]
	public Sprite defaultCursor;
	public Sprite hoverButtonCursor;
	public Sprite turnLeftCursor;
	public Sprite turnRightCursor;
	public Sprite goForwardsCursor;
	public Sprite carveWoodHoverCursor;
	public Sprite carveWoodDownCursor;
	public Sprite drawMapHoverCursor;
	public Sprite drawMapDownCursor;
	public Sprite interactionCursor;

	private Image image;
	private RectTransform rt;
	private RectTransform parentRT;
	private RectTransform canvasRT;

	private void Awake() {
		control = this;
		image = GetComponent<Image>();
		rt = GetComponent<RectTransform>();
		parentRT = transform.parent.GetComponent<RectTransform>();
		canvasRT = parentRT.parent.GetComponent<RectTransform>();
	}

	private void Update() {
		Vector3 mousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRT, Input.mousePosition, null, out mousePos))
			parentRT.position = mousePos;
	}

	public static void ClearAll() {
		turnLeft = false;
		turnRight = false;
		goForwards = false;
		carveWoodHover = false;
		carveWoodDown = false;
		drawMapHover = false;
		drawMapDown = false;
		interaction = false;
		// hoverButton = false;
		control.UpdateCursor();
	}

	private void Start() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
		UpdateCursor();
	}

	private void OnApplicationFocus(bool focusStatus) {
		Cursor.visible = false;
		UpdateCursor();
	}

	private void UpdateCursor() {
		Sprite cursorSprite;
		if (hoverButton) {
			cursorSprite = hoverButtonCursor;
			currentState = CursorState.Hovering;
		} else if (drawMapHover) {
			cursorSprite = drawMapDown ? drawMapDownCursor : drawMapHoverCursor;
			currentState = CursorState.Drawing;
		} else if (turnLeft) {
			cursorSprite = turnLeftCursor;
			currentState = CursorState.Turning;
		} else if (turnRight) {
			cursorSprite = turnRightCursor;
			currentState = CursorState.Turning;
		} else if (carveWoodHover) {
			cursorSprite = carveWoodDown ? carveWoodDownCursor : carveWoodHoverCursor;
			currentState = CursorState.Carving;
		} else if (interaction) {
			cursorSprite = interactionCursor;
			currentState = CursorState.Interaction;
		} else if (goForwards) {
			cursorSprite = goForwardsCursor;
			currentState = CursorState.Forwardsing;
		} else {
			cursorSprite = defaultCursor;
			currentState = CursorState.None;
		}

		image.sprite = cursorSprite;
		if (cursorSprite != null) {
			image.SetNativeSize();
			rt.anchoredPosition = -cursorSprite.pivot;
		}
	}

	public enum CursorState {
		None,
		Turning,
		Forwardsing,
		Carving,
		Drawing,
		Interaction,
		Hovering
	}
}