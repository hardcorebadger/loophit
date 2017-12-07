using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

	private List<GameObject> currentLoops;
	private LineRenderer lineRenderer;
	public float speed;

	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer> ();
		currentLoops = new List<GameObject> ();
	}

	void Update() {

		transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));

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

		GameController.AddPoint ();
		GameObject g = currentLoops [0];
		currentLoops.RemoveAt (0);
		g.SetActive (false);
		return true;
	}

}
