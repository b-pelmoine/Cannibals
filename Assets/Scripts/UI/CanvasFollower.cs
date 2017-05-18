using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollower : MonoBehaviour {

    [SerializeField]
    Canvas canvas;

    public float offset = 0f;

    [SerializeField]
    Transform transformToFollow;

	
	// Update is called once per frame
	void Update () {
        canvas.transform.position = transformToFollow.position + Vector3.up * offset;
        canvas.transform.LookAt(Camera.main.transform);
    }
}
