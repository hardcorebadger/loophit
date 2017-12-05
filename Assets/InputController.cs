using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour {

	public Pointer pointer;
	public GameObject gameOverPopup;

	private bool gameOver = false;

	// Use this for initialization
	void Start () {
		
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

	public static bool OnTap() {
		return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space);
	}
}
