using TMPro;
using UnityEngine;

public class Difficulty : MonoBehaviour {

	private static Difficulty control;

	public static int Level {
		get => level;
		set {
			int length = control?.configs?.Length ?? 0;
			level = (value + length) % length;
			control.UpdateViews();
		}
	}
	private static int level = 1;
	public static DifficultySettings config => control.configs[Level];

	public DifficultySettings[] configs;
	[SerializeField]
	private DifficultyLevel[] levels;
	public Color offColour;
	public Color onColour;
	public Color hoverColour;
	public Color changeColour;
	public Color selectColour;
	public bool useSelect;
	public Sprite offSprite;
	public Sprite onSprite;
	public Sprite removeSprite;
	public Sprite addSprite;
	public TextMeshProUGUI label;
	public TextMeshProUGUI gameViewLabel;

	[HideInInspector]
	public bool[] levelHovers;

	private void Awake() {
		control = this;
		levelHovers = new bool[4];
	}

	private void Start() {
		UpdateViews();
	}

	public void UpdateViews() {
		bool isHovering = false;
		int hoverIndex = 0;
		for (int i = 0; i < levelHovers.Length; i++) {
			if (levelHovers[i]) {
				isHovering = true;
				hoverIndex = i;
				break;
			}
		}

		// game view
		string percent = "";
		for (int i = 0; i < configs.Length; i++)
			percent += i <= Level ? 'x' : '-';
		gameViewLabel.text = $"Difficulty: {config.name} ({percent})";

		label.text = "DIFFICULTY:\n" + configs[isHovering ? hoverIndex : Level].name.ToUpper().Colour(isHovering ? selectColour : Color.white);

		CursorControl.HoverButton = isHovering;

		if (isHovering) {
			for (int i = 0; i < levels.Length; i++) {
				if (i <= Level) {
					levels[i].SetColor(i <= hoverIndex ? hoverColour : changeColour);
					levels[i].SetSprite(i <= hoverIndex ? onSprite : removeSprite);
				} else {
					levels[i].SetColor(i <= hoverIndex ? changeColour : offColour);
					levels[i].SetSprite(i <= hoverIndex ? addSprite : offSprite);
				}
				if (i == hoverIndex && useSelect)
					levels[i].SetColor(selectColour);
			}
		} else {
			for (int i = 0; i < levels.Length; i++) {
				levels[i].SetColor(i <= Level ? onColour : offColour);
				levels[i].SetSprite(i <= Level ? onSprite : offSprite);
			}
		}
	}
}