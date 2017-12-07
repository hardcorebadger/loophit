﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public Pointer pointer;
	public GameObject gameOverPopup;
	public GameObject plugLevel;
	public int difficulty = 1;
	public static int spawnedLevels = 4;
	public static GameController instance;


	private bool gameOver = false;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (OnTap ()) {
			if (!gameOver) {
				if (!pointer.HitLoop ()) {
					gameOver = true;
					gameOverPopup.SetActive (true);
				}
			} else {
				SceneManager.LoadScene("main", LoadSceneMode.Single);
			}
		}
	}

	public static GameObject NextLevel() {
		instance.difficulty++;
		return Instantiate(instance.plugLevel);
	}

	public static bool OnTap() {
		return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space);
	}
}