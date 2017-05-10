using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentManager : MonoBehaviour {

    static public List<GameObject> agents;

	void Awake () {
        agents = new List<GameObject>();
    }

    static public void registerAIAgent(GameObject go)
    {
        agents.Add(go);
    }

    static public List<GameObject> getActiveAgents()
    {
        return agents;
    }

    
}
