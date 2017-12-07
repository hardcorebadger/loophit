using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Pointer pointer;
	public GameObject gameOverPopup;
	public Level plugLevel;
	public Text scoreLabel;

	public Stage[] levelStages;
	public int difficulty = 1;
	public Material[] levelMaterials;
	public bool debugMode = false;

	public static int score = 0;
	public static GameController instance;
	public static Queue<Stage> stages;
	public static Stage currentStage;
	public static int stageLevel = 0;

	private bool gameOver = false;

	// Use this for initialization
	void Start () {
		instance = this;

		stages = new Queue<Stage> ();
		foreach (Stage s in levelStages) {
			stages.Enqueue (s);
		}
		currentStage = stages.Dequeue ();
		GenerateStart ();

	}
	
	// Update is called once per frame
	void Update () {
		if (OnTap ()) {
			if (!gameOver) {
				if (!pointer.HitLoop () && !debugMode) {
					gameOver = true;
					gameOverPopup.SetActive (true);
				}
			} else {
				SceneManager.LoadScene("main", LoadSceneMode.Single);
			}
		}
	}

	public static Level NextLevel() {
		instance.difficulty++;
		IncrementStage ();
		return Instantiate (instance.plugLevel).Generate (
			currentStage, stageLevel, 
			instance.levelMaterials [instance.difficulty % instance.levelMaterials.Length], 5
		);
	}

	private static void IncrementStage() {
		stageLevel++;
		if (stageLevel > currentStage.levels) {
			if (stages.Count > 0) {
				currentStage = stages.Dequeue ();
				stageLevel = 1;
			} else {
				// just stay on last level forever
				stageLevel--;
			}
		}
	}

	public static void AddPoint() {
		GameController.score++;
		instance.scoreLabel.text =  "" + GameController.score;

	}

	public static void SetPointerSpeed(float f) {
		instance.pointer.speed = f;
	}

	void GenerateStart() {
		GameController.score = 0;
		instance.scoreLabel.text =  "" + GameController.score;
		Level genesis = NextLevel ();
		// load genesis, then 4 levels (autolinks)
		genesis.Advance ();genesis.Advance ();genesis.Advance ();genesis.Advance ();
	}

	public static bool OnTap() {
		return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space);
	}


}

[System.Serializable]
public class Stage {
	public int levels, arcs;
	public float startSpeed, endSpeed;
	public float startSize, endSize;
}