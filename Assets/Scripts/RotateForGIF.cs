using UnityEngine;

public class RotateForGIF : MonoBehaviour {

	public float speed;

	private void Update() {
		EnvControl.control.SetDirection(Mathf.InverseLerp(-1, 1, Mathf.Sin(Time.time * speed)));
	}

}