using System.Collections;
using UnityEngine;

public class RandomCrowCaws : MonoBehaviour {

	public bool play;
	public RangeFloat cawFrequency;
	public AudioClip[] caws;
	public RangeFloat volRange;
	public RangeFloat pitchRange;
	public RangeFloat panRange;

	private AudioSource audioSource;

	private IEnumerator Start() {

		audioSource = GetComponent<AudioSource>();

		while (true) {
			while (play) {
				audioSource.pitch = pitchRange.GetRandomFloat();
				audioSource.volume = Extensions.ExpLerp01(volRange.GetRandomFloat());
				audioSource.panStereo = panRange.GetRandomFloat();
				audioSource.PlayOneShot(caws[Random.Range(0, caws.Length)]);
				yield return new WaitForSeconds(cawFrequency.GetRandomFloat());
			}
			yield return null;
		}
	}
}