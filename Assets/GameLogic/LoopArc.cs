using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Loop Arc
 * 
 * This is an atomic class for generating arcs /
 * circle segments of a certain resolution, radius, and theta
 * It generated a mesh and a collider matching the arc
 * using a lineRenderer and an edgeCollider
 * 
 * note* edge collider's have weird properties so if there's
 * issues with it look up the docs for it
 * 
 */

public class LoopArc : MonoBehaviour {

	public float start, end, radius;
	public int segments;

	private LineRenderer lineRenderer;
	private EdgeCollider2D edgeCollider;

	//
	// Object Methods
	//

	// make sure to draw the arc on spawn
	void Start() {
		lineRenderer = GetComponent<LineRenderer> ();
		edgeCollider = GetComponent<EdgeCollider2D> ();
		RefreshArc ();
	}

	public void SetArc(float s, float e, bool refresh = false) {

		start = s;
		end = e;
		if (refresh)
			RefreshArc ();
	}

	// Used mostly in debugging to referesh the arcs from the editor
	// (see LoopArc Editor)
	public void EditorRefresh() {
		lineRenderer = GetComponent<LineRenderer> ();
		edgeCollider = GetComponent<EdgeCollider2D> ();
		RefreshArc ();
	}

	//
	// Helpers
	//

	// The actual trig that draws the arc out of line segments
	private void RefreshArc() {

		// some initial math to trnaslate parameters
		float anglePerSegment = (360f / segments);
		int segmentsSkipStart = (int) (start / anglePerSegment);
		int segmentsSkipEnd = (int) ((360f - end) / anglePerSegment);
		int actualSegments = segments - segmentsSkipStart - segmentsSkipEnd;

		// initialize arrays for renderer and collider
		lineRenderer.positionCount = actualSegments + 1;
		Vector2[] edgePoints = new Vector2[actualSegments + 1];

		float x = 0, y = 0;
		float angle = segmentsSkipStart*anglePerSegment;
		for (int i = 0; i < actualSegments+1; i++) {
			// trig
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

			// set next renderer point
			lineRenderer.SetPosition (i,new Vector3(x,y));
			// set next collider point
			edgePoints [i] = new Vector2 (x, y);

			// increment angle
			angle += (360f / segments);
		}

		// set collider's points to array
		edgeCollider.points = edgePoints;
	}

}
