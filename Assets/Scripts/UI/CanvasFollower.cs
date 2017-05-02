using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollower : MonoBehaviour {

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Transform transformToFollow;

	
	// Update is called once per frame
	void Update () {
        canvas.transform.position = transformToFollow.position;

    }
}
