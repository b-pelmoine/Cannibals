using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentPriorityTest : MonoBehaviour {

    public Vector3 destination;
    public NavMeshAgent agent;
    public Terrain walkable;
	
	// Update is called once per frame
	void FixedUpdate () {
		if(destination == Vector3.zero)
        {
            newDestination();
        }
        else
        {
            float dist = agent.remainingDistance;
            if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
                destination = Vector3.zero;
        }
	}

    public void newDestination()
    {
        Vector3 terrainSize = walkable.terrainData.size;
        destination = new Vector3(50 * Random.value -25, 0, 50 * Random.value -25);
        agent.SetDestination(destination);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white ;
        Gizmos.DrawLine(transform.position, destination);
    }

}
