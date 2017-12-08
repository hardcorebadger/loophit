using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Level
 * 
 * Each level object is responsible for 1 layer of the 
 * vortex - it holds references to all of it's arcs
 * and handles level completion, zooming, and new
 * level spawning as required
 * 
 * Levels are linked in ascending order, with 4 levels
 * being present at any given time ie
 * 
 * 1 -> 2 -> 3 -> 4 -> null
 * 
 * when level (1) is completed, it recursively calls
 * next Level to advance, when next level is null (4)
 * that level is responsible for creating a new level to 
 * fill its place
 * 
 */

public class Level : MonoBehaviour {

	// settings and links
	public GameObject arcPrefab;
	public GameObject[] arcs;
	public float arcSize;
	public float currentArcSize;
	public float zoom;
	public float zoomSpeed;
	public float shrinkSpeed;
	public float speed;

	// private members
	private float currentZoom;
	private int zoomLevel;
	private Level nextLevel;

	//
	// Object Methods
	//

	void Start () {
		// initialize everthing to the right zoom
		SetZoom (currentZoom);
	}
		
	void Update () {
		// check if all the arcs are gone
		bool f = false;
		foreach (GameObject g in arcs) {
			if (g.activeSelf)
				f = true;
		}
		// if they are, the level is over
		if (!f && nextLevel != null) {
			EndLevel ();
		}
		if (this.zoomLevel == 1)
			GameController.instance.currentLevel = this;

		// rotate the level at speed
		transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));
	}

	//
	// Public Interface
	//

	// creates a level given the parameters
	// essentially uses l to do a lerp of the stage start-end params
	public Level Generate(Stage s, int l, Material mat, int zoom) {
		// zome setup math
		float lerpVal = (float)l / (float)s.levels;
		int increment = 360 / s.arcs;
		arcs = new GameObject[s.arcs];

		// initialize the arcs
		for (int i = 0; i < s.arcs; i++) {
			// instantiate
			arcs [i] = Instantiate (arcPrefab, transform);
			// set material
			arcs[i].GetComponent<LineRenderer> ().material = mat;
			// set rotation
			arcs [i].transform.eulerAngles = new Vector3 (0, 0, increment * i);
			arcSize = Mathf.Lerp (s.startSize, s.endSize, lerpVal);
			currentArcSize = arcSize;

			// set size
			arcs [i].GetComponent<LoopArc> ().SetArc (0, arcSize);
		}
		// set speed and zoom to appropriate values
		speed = Mathf.Lerp (s.startSpeed, s.endSpeed, lerpVal);
		zoomLevel = zoom;
		// refresh zoom animated
		SetZoom (GetAppropriateZoom ());
		// randomize start rotation
		transform.eulerAngles = new Vector3(0,0,Random.Range(0f,360f));
		return this;
	}

	//
	// Helpers
	//

	// only called on top level when all arcs are gone
	// ends the level and starts the Advance recursion
	private void EndLevel() {
		nextLevel.Advance();
		Destroy (gameObject);
	}

	// Recursive function to advance (into the vortex)
	public void Advance() {
		// move the level up to next zoom level
		this.zoomLevel--;
		StartCoroutine(Zoom (currentZoom, GetAppropriateZoom ()));

		// if this is the current level, set the pointer to match speed
		if (zoomLevel == 1)
			GameController.SetPointerSpeed (speed * -1);
		if (nextLevel == null) {
			if (zoomLevel == 3) {
				// if we were the last level (4) we are now (3)
				// so it's our turn to generate the next level behind us
				nextLevel = GameController.NextLevel ();
				nextLevel.Advance ();
			}
		} else {
			// otherwise you are in the middle of the stack
			// so nothing important for you to do except advance
			nextLevel.Advance (); 
		}

	}

	// just maps the zoom level to a transform scale value
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

	// actual zoom animation function, takes a transform scale
	// to and scale from and animates between them using zoomSpeed
	// and a coroutine
	private IEnumerator Zoom(float from, float to) {
		if (to > from) {
			while (currentZoom < to) {
				SetZoom(currentZoom+ Time.deltaTime * zoomSpeed);
				if (currentZoom > to) {
					SetZoom (((int)Mathf.Round (to * 10.0f)) / 10f);
				} else {
					yield return null;
				}
			}
		} else {
			while (currentZoom > to) {
				SetZoom(currentZoom- Time.deltaTime * zoomSpeed);
				if (currentZoom < to) {
					SetZoom (((int)Mathf.Round (to * 10.0f)) / 10f);
				} else {
					yield return null;
				}
			}
		}
	}

	// base zoom function that animator uses
	// does a hard set on all the values that need to zoom
	private void SetZoom(float f) {
		currentZoom = f;
		transform.localScale = new Vector3 (currentZoom, currentZoom, currentZoom);
		foreach (GameObject g in arcs) {
			g.GetComponent<LineRenderer> ().widthCurve = AnimationCurve.Linear (0, f, 1, f);
		}
	}

	public void Miss() {
		StartCoroutine (Shrink ());
	}

	private IEnumerator Shrink() {

		if (currentArcSize > 10) {
			float target = currentArcSize - (.5f * arcSize);
			if (target < 10) {
				target = 1;
			}
			while (currentArcSize > target) {
				if (currentArcSize <= 10) {
					GameController.EndGame ();
					Destroy (this.gameObject);
					target = currentArcSize;
					break;
				}
				SetArcSize (currentArcSize - Time.deltaTime * shrinkSpeed);
				if (currentArcSize < target) {
					SetArcSize (target);
				} else {
					yield return null;
				}
			}
			
		} else {
			GameController.EndGame ();
			yield return null;
		}

	}

	private void SetArcSize(float size) {

		foreach (GameObject a in arcs) {
			if (a != null)
				a.GetComponent<LoopArc> ().SetArc (0, size, true);
			
		}
		currentArcSize = size;

	}

}
