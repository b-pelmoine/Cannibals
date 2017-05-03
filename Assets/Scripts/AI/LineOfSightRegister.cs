using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightRegister : MonoBehaviour {

    public int detect_value = 30;
    public MeshRenderer mesh;

    // Use this for initialization
    void Start () {
        LineOfSight.Register(gameObject, detect_value);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
