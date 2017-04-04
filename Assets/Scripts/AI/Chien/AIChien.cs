using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChien : MonoBehaviour {

    public Waypoint patrouille;
    public Vector3 targetPosition;
    int currentWaypoint;
    public float waypointDistance;
    public bool arrived=false;

	// Use this for initialization
	void Start () {
        currentWaypoint = patrouille.getNearest(transform.position);
        targetPosition = patrouille[currentWaypoint];
	}
	
	// Update is called once per frame
	void Update () {
        if ((transform.position - targetPosition).sqrMagnitude < waypointDistance)
        {
            currentWaypoint = patrouille.getNext(currentWaypoint);
            targetPosition = patrouille[currentWaypoint];
            arrived = true;
        }

	}
}
