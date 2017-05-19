using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightRegister : MonoBehaviour {

    public int detect_value = 30;
    public MeshRenderer mesh;
    public bool debug = false;
    public Color col;

    // Use this for initialization
    void Start () {
        if(debug)
            LineOfSight.Register(gameObject, detect_value, col);
        else
            LineOfSight.Register(gameObject, detect_value);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
