using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChien : MonoBehaviour {

    IBlackboard blackboard;
    public GameObject patrouille;
    Waypoint _waypoint;
    public Vector3 targetPosition;
    int currentWaypoint;
    public float waypointDistance;
    public bool arrived=false;
    LineOfSight los;
    public enum btargetType
    {
        Viande, Buisson, Animal
    }
    public btargetType targetType;
    public GameObject target = null;
    public bool sawSomething = false;
	// Use this for initialization
	void Start () {
        _waypoint = patrouille.GetComponent<Waypoint>();
        currentWaypoint = _waypoint.getNearest(transform.position);
        targetPosition = _waypoint[currentWaypoint];
        los = GetComponent<LineOfSight>();
	}
	
	// Update is called once per frame
	void Update () {
        if ((transform.position - targetPosition).sqrMagnitude < waypointDistance)
        {
            currentWaypoint = _waypoint.getNext(currentWaypoint);
            targetPosition = _waypoint[currentWaypoint];
            arrived = true;
        }
        /*if (los!=null && !sawSomething && los.Updated)
        {
            AnalyseSight();
        }*/
	}

    void AnalyseSight()
    {
        for(int i = 0; i < los.sighted.Count; i++)
        {
            if (los.sighted[i].CompareTag("Viande"))
            {
                target = los.sighted[i];
                targetType = btargetType.Viande;
                sawSomething = true;
            }
            else if ((target == null || (targetType==btargetType.Animal && (transform.position- target.transform.position).sqrMagnitude>(transform.position-los.sighted[i].transform.position).sqrMagnitude))
                && los.sighted[i].CompareTag("Animal"))
            {
                target = los.sighted[i];
                targetType = btargetType.Animal;
                sawSomething = true;
            }
        }
    }
}
