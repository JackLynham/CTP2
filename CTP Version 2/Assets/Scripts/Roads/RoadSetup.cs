using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSetup : MonoBehaviour {

    public float pointsDisplaySize = .1f;
    public bool disableEditor;

    public List<Vector3> points;

    public void Init () {
        if (points == null) {
            Reset ();
        }
    }

    public void Reset () {
        points = new List<Vector3> ();
    }

    public void AddPoint (Vector3 p) {
        points.Add (p);
    }
}