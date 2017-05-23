using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererAuto : MonoBehaviour {

	void Start()
    {
        LineRenderer line = GetComponent<LineRenderer>();
        List<Vector3> positions = new List<Vector3>();
        foreach(Transform child in transform)
        {
            positions.Add(child.position);
            Destroy(child.gameObject);
        }
        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
    }
}
