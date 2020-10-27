using UnityEngine;

public class TreeDrawer : MonoBehaviour {

	[Header("//TREE SHIT")]
	public Direction direction;
	public int areaSize = 40;
	[Header("//BRUSH SHIT")]
	public Color shadedColour;
	public Color colour;
	public int brushSize = 2;
	[Header("//AUDIO SHIT")]
	public AudioClip drawSound;
	public float drawSoundAccel;
	public RangeFloat pitchRange;
	public float maxDrawSpeed;

	private Camera mainCam;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	private AudioSource audioSource;
	private Vector4 position;
	private bool mouseInBounds;
	private float audioVelRef;
	private float drawSpeedPercent;
	private float drawIntensity;
	private bool isDrawing;
	private Vector2Int prevPos;

	private void Awake() {
		mainCam = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		audioSource = GetComponent<AudioSource>();

		audioSource.Play();
		audioSource.volume = 0;
	}

	public void ClearTexture() {
		Texture2D tex = spriteRenderer.sprite.texture;
		for (int x = 0; x < tex.width; x++)
			for (int y = 0; y < tex.height; y++)
				tex.SetPixel(x, y, Color.clear);
		tex.Apply();
	}

	public void UpdatePosition(Vector2Int pos) {
		position = new Vector4(pos.x, pos.y, 0, 0);
		position.z = (direction == Direction.East || direction == Direction.West) ? 1 : 0;
		position.w = (direction == Direction.North || direction == Direction.East) ? 1 : 0;
		spriteRenderer.material.SetVector("_Pos", position);
	}

	private void Update() {
		if (WorldControl.gameOver)
			return;

		if (Input.GetMouseButton(0) && mouseInBounds) {

			CursorControl.CarveWoodDown = true;
			Vector2Int pos = GetPixelAtMouse();
			if (Input.GetMouseButtonDown(0))
				DrawAt(pos, true);
			else
				DrawBetween(prevPos, pos);
			prevPos = pos;

		} else if (Input.GetMouseButtonUp(0))
			CursorControl.CarveWoodDown = false;

		float targetVol = isDrawing ? drawSpeedPercent : 0;
		drawIntensity = Mathf.SmoothDamp(drawIntensity, targetVol, ref audioVelRef, drawSoundAccel);
		audioSource.volume = drawIntensity;
		audioSource.pitch = pitchRange.GetAt(drawIntensity);
	}

	private void LateUpdate() {
		isDrawing = false;
	}

	private Vector2Int GetPixelAtMouse() {
		Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
		Vector2 percent = new Vector2(
			Mathf.InverseLerp(boxCollider.bounds.min.x, boxCollider.bounds.max.x, mousePos.x),
			Mathf.InverseLerp(boxCollider.bounds.min.y, boxCollider.bounds.max.y, mousePos.y)
		);

		Vector2Int pixel = new Vector2Int(
			(int)((position.x * areaSize * 2) + (position.z * areaSize) + Mathf.RoundToInt(areaSize * percent.x)),
			(int)((position.y * areaSize * 2) + (position.w * areaSize) + Mathf.RoundToInt(areaSize * percent.y))
		);
		return pixel;
	}

	private void DrawAt(Vector2Int pos, bool apply = false) {
		if (CursorControl.CurrentState != CursorControl.CursorState.Carving)
			return;

		int brushHalfSize = brushSize / 2;

		pos.x = Mathf.Clamp(pos.x - brushHalfSize, 0, spriteRenderer.sprite.texture.width - brushSize);
		pos.y = Mathf.Clamp(pos.y - brushHalfSize, 0, spriteRenderer.sprite.texture.height - brushSize);

		for (int x = pos.x; x < pos.x + brushSize; x++) {
			for (int y = pos.y; y < pos.y + brushSize; y++) {
				if (PixelInBounds(new Vector2Int(x, y))) {
					spriteRenderer.sprite.texture.SetPixel(x, y, y == pos.y ? shadedColour : colour);
					isDrawing = true;
				}
			}
		}
		if (apply)
			spriteRenderer.sprite.texture.Apply();
	}

	private bool PixelInBounds(Vector2Int pos) {
		Vector2Int min = new Vector2Int(
			(int)((position.x * areaSize * 2) + (position.z * areaSize)),
			(int)((position.y * areaSize * 2) + (position.w * areaSize))
		);
		Vector2Int max = min + Vector2Int.one * areaSize;

		return pos.x >= min.x && pos.y >= min.y && pos.x <= max.x && pos.y <= max.y;
	}

	private void DrawBetween(Vector2Int start, Vector2Int end) {
		float distance = Vector2Int.Distance(start, end);
		drawSpeedPercent = Mathf.Clamp01((distance / Time.deltaTime) / maxDrawSpeed);
		Vector2 slope = ((Vector2)(end - start)).normalized;

		Vector2 drawPos = start;
		for (float i = 0; i < distance; i++) {
			drawPos.x += slope.x;
			drawPos.y += slope.y;
			DrawAt(new Vector2Int((int)drawPos.x, (int)drawPos.y));
		}
		spriteRenderer.sprite.texture.Apply();
	}

	private void OnMouseEnter() {
		if (WorldControl.gameOver)
			return;
		mouseInBounds = true;
		CursorControl.CarveWoodHover = true;

		prevPos = GetPixelAtMouse();
	}

	private void OnMouseExit() {
		if (WorldControl.gameOver)
			return;
		mouseInBounds = false;
		CursorControl.CarveWoodHover = false;
	}
}