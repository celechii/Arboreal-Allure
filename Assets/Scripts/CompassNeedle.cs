using UnityEngine;

public class CompassNeedle : MonoBehaviour {
	private void Update() {
		float percent = EnvControl.control.direction / 4f;
		transform.localRotation = Quaternion.Euler(69, 0, Mathf.Lerp(0, 360, percent));
	}
}