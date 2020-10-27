using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaperDrawer : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	public int brushSize;
	public Color drawColour;
	[Header("//AUDIO SHIT")]
	public AudioClip drawSound;
	public float drawSoundAccel;
	public RangeFloat pitchRange;
	public float maxDrawSpeed;

	private CanvasScaler canvasScaler;
	private RectTransform rt;
	private Image canvas;
	private AudioSource audioSource;
	private Texture2D texture;
	private bool isDrawing;
	private Color[] brushColours;
	private Vector2Int prevPos;
	private float audioVelRef;
	private float drawSpeedPercent;
	private float drawIntensity;

	private void Awake() {
		canvasScaler = GetComponentInParent<CanvasScaler>();
		rt = GetComponent<RectTransform>();
		canvas = GetComponent<Image>();
		audioSource = GetComponent<AudioSource>();
		texture = canvas.sprite.texture;
		audioSource.clip = drawSound;

		brushColours = new Color[brushSize * brushSize];
		for (int i = 0; i < brushColours.Length; i++)
			brushColours[i] = drawColour;

		Clear();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Equals))
			Clear();

		float targetVol = isDrawing ? drawSpeedPercent : 0;
		drawIntensity = Mathf.SmoothDamp(drawIntensity, targetVol, ref audioVelRef, drawSoundAccel);
		audioSource.volume = drawIntensity;
		audioSource.pitch = pitchRange.GetAt(drawIntensity);
	}

	private void LateUpdate() {
		isDrawing = false;
	}

	private void Clear() {
		for (int x = 0; x < texture.width; x++)
			for (int y = 0; y < texture.height; y++)
				texture.SetPixel(x, y, Color.clear);
		texture.Apply();
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

		texture.Apply();
	}

	private void DrawAt(Vector2Int pos, bool apply = false) {

		int brushHalfSize = brushSize / 2;

		pos.x = Mathf.Clamp(pos.x - brushHalfSize, 0, texture.width - brushSize);
		pos.y = Mathf.Clamp(pos.y - brushHalfSize, 0, texture.height - brushSize);

		texture.SetPixels(pos.x, pos.y, brushSize, brushSize, brushColours);

		isDrawing = true;

		if (apply)
			texture.Apply();
	}

	private(bool, Vector2Int)GetPixelPosition(PointerEventData data) {
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		Rect rect = Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
		Vector2 percent = new Vector2(Mathf.InverseLerp(rect.min.x, rect.max.x, data.position.x), Mathf.InverseLerp(rect.min.y, rect.max.y, data.position.y));

		return (rect.Contains(data.position), new Vector2Int(Mathf.RoundToInt(texture.width * percent.x), Mathf.RoundToInt(texture.height * percent.y)));
	}

	public void OnPointerDown(PointerEventData data) {

		bool onCanvas;
		Vector2Int pos;
		(onCanvas, pos) = GetPixelPosition(data);
		if (onCanvas)
			DrawAt(pos, true);
		prevPos = pos;
		CursorControl.DrawMapDown = true;
	}

	public void OnDrag(PointerEventData data) {
		bool onCanvas;
		Vector2Int pos;
		(onCanvas, pos) = GetPixelPosition(data);
		if (onCanvas)
			DrawBetween(prevPos, pos);

		prevPos = pos;
	}

	public void OnPointerUp(PointerEventData eventData) {
		CursorControl.DrawMapDown = false;
	}
}