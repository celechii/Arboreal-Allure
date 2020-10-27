using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RestartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public float maxShake;
	public float maxSpacing;
	public float accel;
	[Space]
	public RangeFloat pitchRange;

	private AudioSource audioSource;
	private TextMeshProUGUI text;
	private RectTransform textRT;
	private ScreenShake shake;
	private Vector2 offset;
	private float percent;
	private bool isHovering;
	private float velRef;

	private void Awake() {
		text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		textRT = text.GetComponent<RectTransform>();
		shake = text.GetComponent<ScreenShake>();
		audioSource = GetComponent<AudioSource>();
		offset = textRT.anchoredPosition;
	}

	private void Update() {
		percent = Mathf.SmoothDamp(percent, isHovering ? 1 : 0, ref velRef, accel);
		shake.constantIntensity = maxShake * percent;
		textRT.anchoredPosition = offset + (Vector2)shake.Output;
		text.characterSpacing = maxSpacing * percent;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		isHovering = true;
		CursorControl.HoverButton = true;
		audioSource.pitch = pitchRange.GetRandomFloat();
		audioSource.PlayOneShot(audioSource.clip);
	}

	public void OnPointerExit(PointerEventData eventData) {
		isHovering = false;
		CursorControl.HoverButton = false;
	}
}