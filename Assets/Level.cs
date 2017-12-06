using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	public GameObject[] arcs;
	public Level nextLevel;
	public float zoom;
	public float zoomSpeed;

	// Use this for initialization
	void Start () {
		zoom = transform.localScale.x;
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

	private void EndLevel() {
		nextLevel.ZoomTo (zoom);
		Destroy (gameObject);
	}

	private void ZoomTo(float f) {
		StartCoroutine(Zoom (zoom, f));
		if (nextLevel != null)
			nextLevel.ZoomTo (zoom);
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
		SetZoom (to);
	}

	private void SetZoom(float f) {
		zoom = f;
		transform.localScale = new Vector3 (zoom, zoom, zoom);
		foreach (GameObject g in arcs) {
			g.GetComponent<LineRenderer> ().widthCurve = AnimationCurve.Linear (0, f, 1, f);
		}
	}
}
