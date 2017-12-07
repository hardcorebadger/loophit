using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Pointer pointer;
	public GameObject gameOverPopup;
	public Level plugLevel;
	public int difficulty = 1;
	public Material[] levelMaterials;
	public int stageLength = 5;
	public int maxArcs = 6;
	public Text scoreLabel;

	public static int score = 0;
	public static int spawnedLevels = 4;
	public static GameController instance;

	private bool gameOver = false;

	// Use this for initialization
	void Start () {
		instance = this;

		GenerateStart ();
	}
	
	// Update is called once per frame
	void Update () {
		if (OnTap ()) {
			if (!gameOver) {
				if (!pointer.HitLoop ()) {
//					gameOver = true;
//					gameOverPopup.SetActive (true);
				}
			} else {
				SceneManager.LoadScene("main", LoadSceneMode.Single);
			}
		}
	}

	public static Level NextLevel() {
		instance.difficulty++;
		return Instantiate(instance.plugLevel)
			.Create(5, instance.difficulty + spawnedLevels - 1)
			.Generate(instance.levelMaterials[(instance.difficulty+spawnedLevels) % instance.levelMaterials.Length]);

	}

	public static void AddPoint() {
		GameController.score++;
		instance.scoreLabel.text =  "" + GameController.score;

	}

	public static void SetPointerSpeed(float f) {
		instance.pointer.speed = f;
	}

	void GenerateStart() {

		instance.scoreLabel.text =  "" + GameController.score;
		Level a = Instantiate(instance.plugLevel)
			.Create(1, 1)
			.Generate(instance.levelMaterials[1 % instance.levelMaterials.Length]);
		Level b = Instantiate(instance.plugLevel)
			.Create(2, 2)
			.Generate(instance.levelMaterials[2 % instance.levelMaterials.Length]);
		Level c = Instantiate(instance.plugLevel)
			.Create(3, 3)
			.Generate(instance.levelMaterials[3 % instance.levelMaterials.Length]);
		Level d = Instantiate(instance.plugLevel)
			.Create(4, 4)
			.Generate(instance.levelMaterials[4 % instance.levelMaterials.Length]);

		a.nextLevel = b;
		b.nextLevel = c;
		c.nextLevel = d;
	}

	public static bool OnTap() {
		return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space);
	}
}
