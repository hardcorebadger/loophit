using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Game Controller
 * 
 * Static controller class for the game
 * handles level and stage incrementing, score, 
 * and initial level generation
 * 
 */

public class GameController : MonoBehaviour {

	// Game Settings and Links
	public Pointer pointer;
	public GameObject gameOverPopup;
	public Level plugLevel;
	public Text scoreLabel;
	public Stage[] levelStages;
	public Material[] levelMaterials;
	public bool debugMode = false;
	public Level currentLevel;

	// static members
	public static int score = 0;
	public static GameController instance;
	private static Queue<Stage> stages;
	private static Stage currentStage;
	private static int stageLevel = 0;
	private static int globalLevel = 0;
	private static bool gameOver = false;

	//
	// Object Methods
	//

	private void Start () {
		
		// link static instance
		instance = this;

		// initialize stage queue
		stages = new Queue<Stage> ();
		foreach (Stage s in levelStages) {
			stages.Enqueue (s);
		}

		// re-initialize the game (for restarting)
		stageLevel = 0;
		globalLevel = 0;
		gameOver = false;
		SetScore (0);
		currentStage = stages.Dequeue ();

		// generate starting levels
		InitializeLevels ();
	}
	
	private void Update () {
		if (OnTap ()) {
			// reload the game
			if (gameOver) {
				SceneManager.LoadScene ("main", LoadSceneMode.Single);
				return;
			}

			if (pointer.HitLoop ()) {
				// give a point
				AddPoint ();
			} else if (!debugMode) {
				// game over
				currentLevel.Miss ();
			}

		}
	}

	// 
	// Public Interface
	//

	// returns a newly generated level (called by lowest level advancing)
	public static Level NextLevel() {
		IncrementStage ();
		return Instantiate (instance.plugLevel).Generate (
			currentStage, stageLevel, 
			instance.levelMaterials [(globalLevel-1) % instance.levelMaterials.Length], 5
		);
	}

	// sets the pointer's speed (called by current level)
	public static void SetPointerSpeed(float f) {
		instance.pointer.speed = f;
	}

	//
	// Helpers
	//

	// generates the first 4 levels on start
	private static void InitializeLevels() {
		SetScore (0);
		Level genesis = NextLevel ();
		// load genesis, then 4 levels (autolinks)
		genesis.Advance ();genesis.Advance ();genesis.Advance ();genesis.Advance ();
	}

	// detects a tap or space bar hit for testing
	private static bool OnTap() {
		return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space);
	}

	// advances stage to next value, handles queue if need be
	private static void IncrementStage() {
		globalLevel++;
		stageLevel++;
		// if we're at the end of the stage
		if (stageLevel > currentStage.levels) {
			// try to dequeue the next stage
			if (stages.Count > 0) {
				currentStage = stages.Dequeue ();
				stageLevel = 1;
			} else {
				// just stay on last level forever (temp)
				stageLevel--;
			}
		}
	}

	// adds a point to the score 
	private static void AddPoint() {
		GameController.score++;
		instance.scoreLabel.text =  "" + GameController.score;

	}

	// adds a point to the score 
	private static void SetScore(int i) {
		GameController.score = i;
		instance.scoreLabel.text =  "" + GameController.score;
	}

	public static void EndGame() {
		gameOver = true;
		instance.gameOverPopup.SetActive (true);
	}

}

// data type for level editor
[System.Serializable]
public class Stage {
	public int levels, arcs;
	public float startSpeed, endSpeed;
	public float startSize, endSize;
}