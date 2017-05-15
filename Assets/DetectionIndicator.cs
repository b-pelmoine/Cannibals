using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionIndicator : MonoBehaviour {

    public GameObject[] players;
    private List<Transform> playerTransforms;
    private List<GameObject> playerMeshGO;

	void Awake() {
        //find the player transforms
        foreach (GameObject go in players)
            playerTransforms.Add(go.transform.Find("NotStatic").transform);
        //find the player mesh gameObject owners
        foreach (Transform t in playerTransforms)
            playerMeshGO.Add(t.Find("cannibal").gameObject);
    }
	
    public void UpdateTrackersList()
    {

    }
	
}
