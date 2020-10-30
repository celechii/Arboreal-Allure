using UnityEngine;

public class MapControl : MonoBehaviour {

	public static MapControl control;

	[Header("//MAP SHIT")]
	public Vector2Int mapSize;
	public Vector2Int position;
	public Vector2Int gearPos;
	public Vector2Int bridgePos;
	public Direction bridgeDir;
	public int startOffsetMaxDist;
	public float gearMinDist;
	[Header("//PERLIN SHIT")]
	public bool drawGizmos;
	[Range(0, 1)]
	public float offset;
	public AnimationCurve progressCurve;
	public Vector2 perlinOffset;
	public float perlinScale;
	[Header("//AUDIO SHIT")]
	public AudioClip roomTransitionSFX;
	public RangeFloat transitionPitch;
	[Header("//REFERENCE SHIT")]
	public TreeDrawer[] treeDrawers;
	public Transform signs;
	public Transform gearMachine;
	public RectTransform gearLocationRT;
	public RectTransform playerStartPosRT;
	public EnvControl envControl;
	public Timer timer;

	private AudioSource audioSource;
	private float roomStartScarecrowLevel;
	private float[, ] scarecrowZone;

	private void Awake() {
		control = this;
		audioSource = GetComponent<AudioSource>();
	}

	private void Start() {
		Setup();
	}

	private void Update() {
		if (roomStartScarecrowLevel < 1 && GetScarecrowZoneLevel() >= 1) {
			roomStartScarecrowLevel = GetScarecrowZoneLevel();
			DeathBar.control.StartBar();
		}

		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.I))
			print(GetScarecrowZoneLevel());
		#endif
	}

	public float GetScarecrowZoneLevel() => GetScarecrowZoneLevel(position);

	public float GetScarecrowZoneLevel(Vector2Int pos) {
		float val = scarecrowZone[pos.x, pos.y] + progressCurve.Evaluate(1 - timer.PercentDeathPeriod);
		if (pos == bridgePos)
			val = Mathf.Clamp(val, 0, .95f);
		return val;
	}

	public bool IsScarecrowZone() => IsScarecrowZone(position);

	public bool IsScarecrowZone(Vector2Int pos) => GetScarecrowZoneLevel(pos) >= 1;

	private void SetBridgePos() {
		Vector2Int idealPos = new Vector2Int();
		float closestDist = 100;
		for (int x = 0; x < mapSize.x; x++) {
			for (int y = 0; y < mapSize.y; y++) {
				if (x == 0 || y == 0 || x == mapSize.x - 1 || y == mapSize.y - 1) {
					float dist = Mathf.Abs(scarecrowZone[x, y] - Difficulty.config.targetExitDangerLevel);
					if (dist < closestDist) {
						closestDist = dist;
						idealPos.x = x;
						idealPos.y = y;
					}
				}
			}
		}

		bridgePos = idealPos;

		if (bridgePos.x == 0)
			bridgeDir = Direction.West;
		else if (bridgePos.y == 0)
			bridgeDir = Direction.South;
		else if (bridgePos.x == mapSize.x - 1)
			bridgeDir = Direction.East;
		else if (bridgePos.y == mapSize.y - 1)
			bridgeDir = Direction.North;
		float normalizedPosition = ((int)bridgeDir / 3f) * 4f;
	}

	private void OnValidate() {
		GenerateScarecrowZone();
	}

	private void GenerateScarecrowZone() {
		scarecrowZone = new float[mapSize.x, mapSize.y];
		perlinOffset = new Vector2(Random.value, Random.value) * 999f;
		float minVal = 1;
		float maxVal = 0;
		for (int x = 0; x < mapSize.x; x++) {
			for (int y = 0; y < mapSize.y; y++) {
				float val = Mathf.PerlinNoise(x * perlinScale + perlinOffset.x, y * perlinScale + perlinOffset.y);
				scarecrowZone[x, y] = val;
				if (val > maxVal)
					maxVal = val;
				if (val < minVal)
					minVal = val;
			}
		}

		for (int x = 0; x < mapSize.x; x++)
			for (int y = 0; y < mapSize.y; y++)
				scarecrowZone[x, y] = Mathf.InverseLerp(minVal, maxVal, scarecrowZone[x, y]) - .001f;
	}

	public void Setup() {

		GenerateScarecrowZone();
		SetBridgePos();

		position = Vector2Int.one * 7;

		do {
			gearPos = new Vector2Int(Random.Range(1, mapSize.x - 1), Random.Range(1, mapSize.y - 1));
		} while (Vector2Int.Distance(position, gearPos) < gearMinDist && scarecrowZone[gearPos.x, gearPos.y] >= .7f && scarecrowZone[gearPos.x, gearPos.y] <= .3f);

		Vector2Int halfMap = mapSize / 2;

		gearLocationRT.pivot = new Vector2(
			gearPos.x >= halfMap.x ? 0 : 1,
			gearPos.y >= halfMap.y ? 0 : 1
		);
		gearLocationRT.anchoredPosition = Vector2.zero;

		position.x += (gearLocationRT.pivot.x == 0 ? -1 : 1) * (Random.Range(1, halfMap.x - 1));
		position.y += (gearLocationRT.pivot.y == 0 ? -1 : 1) * (Random.Range(1, halfMap.y - 1));

		playerStartPosRT.anchoredPosition = position * 32;

		treeDrawers[0]?.ClearTexture();
		UpdateTreePositions();
		BuildRoom();

		signs.localPosition = new Vector2(envControl.GetPosition((int)bridgeDir, 3), signs.localPosition.y);
		gearMachine.localPosition = Vector2.right * (envControl.GetPosition((int)bridgeDir, 2) - 10);
	}

	public void Move(Direction dir) {
		switch (dir) {
			case Direction.North:
				position += Vector2Int.up;
				break;
			case Direction.East:
				position += Vector2Int.right;
				break;
			case Direction.South:
				position += Vector2Int.down;
				break;
			case Direction.West:
				position += Vector2Int.left;
				break;
		}

		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.LeftShift))
			position = gearPos;
		#endif

		UpdateTreePositions();
		audioSource.pitch = transitionPitch.GetRandomFloat();
		audioSource.PlayOneShot(roomTransitionSFX);
	}

	public void BuildRoom() {
		WallType[] walls = new WallType[4];

		if (position.x == 0)
			walls[3] = WallType.DeadEnd;
		if (position.y == 0)
			walls[2] = WallType.DeadEnd;
		if (position.x == mapSize.x - 1)
			walls[1] = WallType.DeadEnd;
		if (position.y == mapSize.y - 1)
			walls[0] = WallType.DeadEnd;

		if (position == bridgePos)
			walls[(int)bridgeDir] = WallType.Bridge;

		roomStartScarecrowLevel = GetScarecrowZoneLevel();
		envControl.BuildRoom(walls, position == gearPos, IsScarecrowZone());
	}

	private void UpdateTreePositions() {
		for (int i = 0; i < treeDrawers.Length; i++)
			treeDrawers[i].UpdatePosition(position);
	}

	private void OnDrawGizmos() {
		if (drawGizmos) {
			for (int x = 0; x < mapSize.x; x++) {
				for (int y = 0; y < mapSize.y; y++) {
					float value = scarecrowZone[x, y];
					if (Application.isPlaying)
						value = GetScarecrowZoneLevel(new Vector2Int(x, y));
					if (value >= 1)
						Gizmos.color = Color.red;
					else
						Gizmos.color = new Color(value, value, value, 1);
					Gizmos.DrawCube(new Vector2(x, y), Vector2.one);
				}
			}
		}
	}
}

public enum Direction {
	North,
	East,
	South,
	West
}

public enum WallType {
	Path,
	DeadEnd,
	Bridge
}