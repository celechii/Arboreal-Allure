using UnityEngine;

public class EnvLayer : MonoBehaviour {

	[Header("//REFERENCE SHIT")]
	public SpriteRenderer[] renderers;

	public RangeFloat posRange = new RangeFloat();

	public void SetDirection(float direction) {
		posRange.SetValueToPercent(direction / 4f);
		transform.localPosition = Vector2.left * posRange.Value;
	}

	public void SetWalls(Sprite[] sprites, float direction) {

		float offset = 0;
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].sprite = sprites[i];
			renderers[i].transform.localPosition = Vector2.right * offset;
			offset += GetSpriteWidth(renderers[i].sprite);
		}
		posRange = new RangeFloat(GetSpriteWidth(sprites[0]) / 2f, offset - (GetSpriteWidth(sprites[3]) / 2f), direction / 4f);
		SetDirection(direction);
	}

	public void ReplaceWall(int index, Sprite newWall) => renderers[index].sprite = newWall;

	private float GetSpriteWidth(Sprite sprite) => sprite.texture.width / sprite.pixelsPerUnit;
}