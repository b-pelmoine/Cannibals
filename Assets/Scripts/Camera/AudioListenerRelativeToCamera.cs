using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerRelativeToCamera : MonoBehaviour {

    private Transform parent;
    private FollowCam camScript;

    [Range(2f, 20f)]
    public float factor;

    // Use this for initialization
    void Start () {
        parent = transform.parent;
        camScript = parent.GetComponent<FollowCam>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = camScript.getPlayersBarycenter() + Vector3.up * parent.transform.position.y/factor;
	}
}
