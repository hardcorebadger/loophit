using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopArc : MonoBehaviour {

	public float start, end, radius;
	public int segments;

	private LineRenderer lineRenderer;
	private EdgeCollider2D edgeCollider;

	void Start() {
		lineRenderer = GetComponent<LineRenderer> ();
		edgeCollider = GetComponent<EdgeCollider2D> ();
		RefreshArc ();
	}

	public void SetArc(float s, float e) {
		start = s;
		end = e;
	}

	private void RefreshArc() {

		float anglePerSegment = (360f / segments);
		int segmentsSkipStart = (int) (start / anglePerSegment);
		int segmentsSkipEnd = (int) ((360f - end) / anglePerSegment);
		int actualSegments = segments - segmentsSkipStart - segmentsSkipEnd;
		
		lineRenderer.positionCount = actualSegments + 1;
		Vector2[] edgePoints = new Vector2[actualSegments + 1];

		float x = 0, y = 0;
		float angle = segmentsSkipStart*anglePerSegment;
		for (int i = 0; i < actualSegments+1; i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

			lineRenderer.SetPosition (i,new Vector3(x,y));
			edgePoints [i] = new Vector2 (x, y);

			angle += (360f / segments);
		}

		edgeCollider.points = edgePoints;
	}

	public void EditorRefresh() {
		lineRenderer = GetComponent<LineRenderer> ();
		edgeCollider = GetComponent<EdgeCollider2D> ();
		RefreshArc ();
	}

}
