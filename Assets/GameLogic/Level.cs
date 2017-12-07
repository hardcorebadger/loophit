﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	public GameObject arcPrefab;
	public GameObject[] arcs;
	public Level nextLevel;
	public float zoom;
	public float zoomSpeed;
	public int zoomLevel, difficulty;
	public float speed;

	// Use this for initialization
	void Start () {
		SetZoom (zoom);
	}
	
	// Update is called once per frame
	void Update () {
		bool f = false;
		foreach (GameObject g in arcs) {
			if (g.activeSelf)
				f = true;
		}
		if (!f && nextLevel != null) {
			EndLevel ();
		}
		transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));

	}

	public Level Create(int zoom, int difficulty) {
		this.zoomLevel = zoom;
		this.difficulty = difficulty;
		return this;
	}

	public Level Generate(Material mat) {
		int arcAmount = Mathf.Min(GameController.instance.maxArcs, ((difficulty / (GameController.instance.stageLength+1)) + 1));
		int increment = 360 / arcAmount;
		arcs = new GameObject[arcAmount];
		for (int i = 0; i < arcAmount; i++) {
			arcs [i] = Instantiate (arcPrefab, transform);
			arcs[i].GetComponent<LineRenderer> ().material = mat;
			arcs [i].transform.eulerAngles = new Vector3 (0, 0, increment * i);
			arcs [i].GetComponent<LoopArc> ().SetArc (0, GetSize(difficulty, increment));
		}

		speed = 90 + difficulty*5f;
		SetZoom (GetAppropriateZoom ());
		return this;
	}

	private int GetSize(int diff, int max) {
		int stage = Mathf.Min(GameController.instance.maxArcs, ((diff / (GameController.instance.stageLength+1)) + 1));
		int cappedDiff = Mathf.Min (diff, GameController.instance.maxArcs * GameController.instance.stageLength);
		int localDiff = cappedDiff - ((stage-1) * GameController.instance.stageLength);
		float percent = ((float)localDiff) / ((float)GameController.instance.maxArcs);
		return Mathf.Max(15, (int)(max - (percent*max)));
	}

	private void EndLevel() {
		nextLevel.Advance();
		Destroy (gameObject);
	}

	private void Advance() {
		this.zoomLevel--;
		float f = GetAppropriateZoom ();
		StartCoroutine(Zoom (zoom, f));
		if (zoomLevel == 1)
			GameController.SetPointerSpeed (speed * -1);
		if (nextLevel == null) {
			if (zoomLevel == 3) {
				nextLevel = GameController.NextLevel ();
				nextLevel.Advance ();
			}
		} else {
			nextLevel.Advance (); 
		}

	}

	private float GetAppropriateZoom() {
		float f = 0.1f;
		switch (zoomLevel) {
		case 1:
			f = 1;
			break;
		case 2:
			f = 0.6f;
			break;
		case 3:
			f = 0.3f;
			break;
		case 4: 
			f = 0.1f;
			break;

		}
		return f;
	}

	private IEnumerator Zoom(float from, float to) {
		if (to > from) {
			while (zoom < to) {
				SetZoom(zoom+ Time.deltaTime * zoomSpeed);
				yield return null;
			}
		} else {
			while (zoom > to) {
				SetZoom(zoom- Time.deltaTime * zoomSpeed);
				yield return null;
			}
		}
		SetZoom (((int)Mathf.Round(to * 10.0f)) / 10f);
	}

	private void SetZoom(float f) {
		zoom = f;
		transform.localScale = new Vector3 (zoom, zoom, zoom);
		foreach (GameObject g in arcs) {
			g.GetComponent<LineRenderer> ().widthCurve = AnimationCurve.Linear (0, f, 1, f);
		}
	}
}