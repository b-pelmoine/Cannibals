using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnaround : MonoBehaviour {
    [Range(0f, 20f)]
	public float speed =5f;
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, Time.deltaTime*speed);
	}
}
