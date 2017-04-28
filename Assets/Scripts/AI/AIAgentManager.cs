using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentManager : MonoBehaviour {

    private List<GameObject> agents;

	void Awake () {
        agents = new List<GameObject>();
    }

    public void registerAIAgent(GameObject go)
    {
        agents.Add(go);
    }

    public List<GameObject> getActiveAgents()
    {
        return agents;
    }

    
}
