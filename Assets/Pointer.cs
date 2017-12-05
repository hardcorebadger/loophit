using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

	private List<GameObject> currentLoops;
	private LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer> ();
		currentLoops = new List<GameObject> ();
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (c.tag == "loop")
			currentLoops.Add(c.gameObject);
	}

	void OnTriggerExit2D (Collider2D c) {
		if (c.tag == "loop")
			currentLoops.Remove(c.gameObject);
	}

	public bool HitLoop() {
		if (currentLoops.Count <= 0)
			return false;
		GameObject g = currentLoops [0];
		currentLoops.RemoveAt (0);
		g.SetActive (false);
		return true;
	}

}
