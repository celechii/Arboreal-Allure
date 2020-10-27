using UnityEngine;

public class PlayerIcon : MonoBehaviour {

	private Transform gearIcon;

	private void Awake() {
		gearIcon = transform.GetChild(0);
	}

	private void Update() {
		transform.localPosition = (Vector2)MapControl.control.position;
		gearIcon.position = (Vector2)MapControl.control.gearPos;
	}

}