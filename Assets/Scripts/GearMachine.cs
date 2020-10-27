using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GearMachine : MonoBehaviour {

	public static bool isInBridgeArea;
	public static bool isHovering { get { return hovers[0] || hovers[1]; } }
	public static bool[] hovers = new bool[2];

	public LayerMask machineMask;
	public float accel;
	[Range(0, 1)]
	public int index;
	public float waitAfterWin;
	public CanvasGroup canvasGroup;
	public UnityEvent ImmediateOnWin;
	public UnityEvent OnWin;

	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;
	private AudioSource audioSource;
	private Camera mainCam;
	private float opacity;
	private float velRef;

	private void Awake() {
		mainCam = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		audioSource = GetComponent<AudioSource>();
	}

	private void Update() {

		boxCollider.enabled = isInBridgeArea;

		Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
		hovers[index] = Physics2D.Raycast(ray.origin, ray.direction, 50f, machineMask);

		if (index == 0) {
			opacity = Mathf.SmoothDamp(opacity, isHovering && isInBridgeArea && !Gear.hasGear ? 1 : 0, ref velRef, accel);
			canvasGroup.alpha = opacity;

			if (isInBridgeArea && Gear.hasGear && !WorldControl.gameOver)
				CursorControl.Interaction = isHovering;
		}
		Color spriteColour = new Color(1, 1, 1, isHovering && isInBridgeArea && Gear.hasGear && !WorldControl.gameOver ? 1 : 0);
		spriteRenderer.color = spriteColour;

		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.W)) {
			isInBridgeArea = true;
			Gear.hasGear = true;
			StartCoroutine(OnMouseUpAsButton());
		}
		#endif

	}

	private IEnumerator OnMouseUpAsButton() {
		if (!isInBridgeArea || !Gear.hasGear || WorldControl.gameOver)
			yield break;
		audioSource.Play();
		EnvControl.control.ReplaceWallWithGear();
		WorldControl.gameOver = true;
		ImmediateOnWin.Invoke();
		yield return new WaitForSeconds(waitAfterWin);
		OnWin.Invoke();
	}

}