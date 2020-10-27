using UnityEngine;

public class BlockPath : MonoBehaviour {

	public static bool block { get { return blocks[0] || blocks[1]; } }
	private static bool[] blocks = new bool[2];

	public LayerMask blockLayer;
	[Range(0, 1)]
	public int index;

	private Camera mainCam;

	private void Awake() {
		mainCam = Camera.main;
	}

	private void Update() {
		Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
		blocks[index] = Physics2D.Raycast(ray.origin, ray.direction, 50f, blockLayer);
	}
}