using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Pointer
 * 
 * This is the behaviour for the cursor/pointer
 * which spins the oppiste direction of the levels
 * Essentially it just rotates and keeps track of 
 * currently colliding arcs to determine a hit vs a miss
 * 
 * Hit loop is called on tap by Game Controller
 * 
 */
public class Pointer : MonoBehaviour {

	public float speed;

	// currently overlapped arcs
	private List<GameObject> currentArcs;

	//
	// Object Methods
	//

	private void Start () {
		currentArcs = new List<GameObject> ();
	}

	private void Update() {
		transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));
	}

	//
	// Event Hooks
	//

	private void OnTriggerEnter2D(Collider2D c) {
		if (c.tag == "arc")
			currentArcs.Add(c.gameObject);
	}

	private void OnTriggerExit2D (Collider2D c) {
		if (c.tag == "arc")
			currentArcs.Remove(c.gameObject);
	}

	//
	// Public Interface
	//

	// called on GameController OnTap
	// decides if it was a hit or a miss
	public bool HitLoop() {
		if (currentArcs.Count <= 0)
			return false;

		GameObject g = currentArcs [0];
		currentArcs.RemoveAt (0);
		g.SetActive (false);
		return true;
	}

}
