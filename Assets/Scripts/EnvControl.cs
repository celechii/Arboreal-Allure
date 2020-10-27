using UnityEngine;

public class EnvControl : MonoBehaviour {

	public static EnvControl control;

	[Header("//MOVEMENT SHIT")]
	[Range(0, 4)]
	public float direction;
	public float moveSpeed;
	public float regularAccel;
	public float anxiousAccel;
	public float anxiousMultiplier;
	public bool isAnxious;
	[Header("//GEAR SHIT")]
	public GameObject gear1;
	public GameObject gear2;
	public Vector2 gearOffset;
	[Header("//ROOM SHIT")]
	public EnvLayer[] layers;
	public BoxCollider2D[] pathColliders;
	public LayerSprites[] layerSprites;
	public Sprite gearAddedWallSprite;
	[Space]
	public GameObject signs;

	private float velRef;
	private float currentVel;
	private float targetVel;
	private WallType[] currentWalls;

	private void Awake() {
		control = this;
		UpdateEnv();
	}

	private void Update() {
		targetVel = 0;
	}

	private void LateUpdate() {
		SetDirection(direction, true);
	}

	private void FixedUpdate() {
		currentVel = Mathf.SmoothDamp(currentVel, targetVel, ref velRef, isAnxious ? anxiousAccel : regularAccel, Mathf.Infinity, Time.fixedDeltaTime);
		direction += currentVel;
	}

	// this is the player one
	public void Rotate(float dir) {
		if (!Scarecrow.isBeingScary && !WorldControl.gameOver)
			Turn(dir);
	}

	public void Turn(float dir) {
		targetVel = moveSpeed * (isAnxious ? anxiousMultiplier : 1) * dir;
	}

	public void SetDirection(Direction direction) => SetDirection((float)direction);

	public void SetDirection(float directionValue, bool ignoreAnxiety = false) {
		if (isAnxious && !ignoreAnxiety)
			directionValue += (Random.value >.5f) ? -.5f : .5f;

		direction = (directionValue + 4) % 4;
		UpdateEnv();
	}

	private void UpdateEnv() {
		for (int i = 0; i < layers.Length; i++)
			if (layers[i].gameObject.activeSelf)
				layers[i].SetDirection(direction);
	}

	public float GetPosition(int layer) => GetPosition(direction, layer);

	public float GetPosition(float direction, int layer) {
		return layers[layer].posRange.GetAt(direction / 4);
	}

	public void ReplaceWallWithGear() {
		for (int i = 0; i < currentWalls.Length; i++) {
			if (currentWalls[i] == WallType.Bridge) {
				layers[2].ReplaceWall(i, gearAddedWallSprite);
				return;
			}
		}
	}

	public void BuildRoom(WallType[] walls, bool hasGear, bool scarecrow) {
		for (int i = 0; i < walls.Length; i++)
			pathColliders[i].enabled = walls[i] == WallType.Path;
		currentWalls = walls;

		pathColliders[4].enabled = pathColliders[0].enabled;

		Sprite[] sprites = new Sprite[5];

		bool hasBridge = false;
		for (int i = 0; i < layers.Length; i++) {
			for (int y = 0; y < walls.Length; y++) {
				if (walls[y] == WallType.Path)
					sprites[y] = layerSprites[i].path;
				else if (walls[y] == WallType.DeadEnd)
					sprites[y] = layerSprites[i].deadEnd;
				else if (walls[y] == WallType.Bridge) {
					sprites[y] = layerSprites[i].bridge;
					hasBridge = true;
				}
			}
			sprites[4] = sprites[0];
			layers[i].SetWalls(sprites, direction);
		}

		signs.SetActive(hasBridge);
		GearMachine.isInBridgeArea = hasBridge;

		if (hasGear) {
			gear1.gameObject.SetActive(true);
			gear2.gameObject.SetActive(true);
			if (!Gear.hasGear)
				gear1.transform.position = gearOffset;
		} else {
			gear1.gameObject.SetActive(false);
			gear2.gameObject.SetActive(false);
		}
	}

	[System.Serializable]
	public struct LayerSprites {
		public Sprite path;
		public Sprite deadEnd;
		public Sprite bridge;
	}
}