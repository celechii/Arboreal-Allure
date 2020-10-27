using UnityEngine;

public class PathExit : MonoBehaviour {

	public Direction direction;
	public Color defaultColour;
	public Color hoverColour;
	[Space]
	public float regularTransition;
	public float anxiousTransition;

	private SpriteRenderer spriteRenderer;
	private bool hover;

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnMouseEnter() {
		if (WorldControl.gameOver || BlockPath.block)
			return;
		hover = true;
		spriteRenderer.color = hoverColour;
		CursorControl.GoForwards = true;
	}

	private void OnMouseExit() {
		if (WorldControl.gameOver)
			return;
		hover = false;
		spriteRenderer.color = defaultColour;
		CursorControl.GoForwards = false;
	}

	private void OnMouseUpAsButton() {
		if (CursorControl.CurrentState != CursorControl.CursorState.Forwardsing || ScreenTransition.isTransitioning || WorldControl.gameOver || Scarecrow.isBeingScary || BlockPath.block)
			return;

		ScreenTransition.Transition(EnvControl.control.isAnxious ? anxiousTransition : regularTransition, () => {
			MapControl.control.Move(direction);
			EnvControl.control.SetDirection(direction);
			MapControl.control.BuildRoom();
			DeathBar.control.NewRoom(MapControl.control.IsScarecrowZone());
		});
	}
}