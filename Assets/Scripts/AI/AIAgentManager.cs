using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentManager : MonoBehaviour {

    public GameObject testAgentPrefab;
    public Material agentMaterial;
    public Terrain walkable;

    private List<GameObject> agents;

	void Start () {
        agents = new List<GameObject>();
		for(int i=0; i<0; i++)
        {
            GameObject instance = Instantiate(testAgentPrefab, transform);
           
            AIAgentPriorityTest script = instance.GetComponent<AIAgentPriorityTest>();
            LevelOfImportance level = instance.GetComponent<LevelOfImportance>();
            level.setLevel(Mathf.RoundToInt(Random.Range(1,10)));
            script.agent = instance.GetComponent<NavMeshAgent>();
            script.walkable = walkable;
            script.destination = Vector3.zero;

            agents.Add(instance);
        }
        
    }

    
}
