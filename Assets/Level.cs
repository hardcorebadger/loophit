using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	public GameObject arcPrefab;
	public GameObject[] arcs;
	public Level nextLevel;
	public float zoom;
	public float zoomSpeed;

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
	}

	public Level Generate(int difficulty, Material mat) {

		int arcAmount = Mathf.Min(GameController.instance.maxArcs, ((difficulty / (GameController.instance.stageLength+1)) + 1));
		int increment = 360 / arcAmount;
		arcs = new GameObject[arcAmount];
		for (int i = 0; i < arcAmount; i++) {
			arcs [i] = Instantiate (arcPrefab, transform);
			arcs[i].GetComponent<LineRenderer> ().material = mat;
			arcs [i].transform.eulerAngles = new Vector3 (0, 0, increment * i);
			arcs [i].GetComponent<LoopArc> ().SetArc (0, GetSize(difficulty, increment));
		}

		GetComponent<Test> ().speed = 90 + difficulty*5f;
		SetZoom (zoom);
		return this;
	}

	private int GetSize(int diff, int max) {
		int stage = Mathf.Min(GameController.instance.maxArcs, ((diff / (GameController.instance.stageLength+1)) + 1));
		int cappedDiff = Mathf.Min (diff, GameController.instance.maxArcs * GameController.instance.stageLength);
		int localDiff = cappedDiff - ((stage-1) * GameController.instance.stageLength);
		return Mathf.Max(1, ((int)(max - ((localDiff +1) * (0.05f*stage))*max)));
	}

	private void EndLevel() {
		nextLevel.ZoomTo (zoom, true);
		Destroy (gameObject);
	}

	private void ZoomTo(float f, bool canSpawn) {
		StartCoroutine(Zoom (zoom, f));
		if (nextLevel == null) {
			if (canSpawn) {
				nextLevel = GameController.NextLevel ();
				nextLevel.ZoomTo (zoom, false);
			}
		} else {
			nextLevel.ZoomTo (zoom, true);
		}
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
