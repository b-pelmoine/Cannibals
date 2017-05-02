using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentManager : MonoBehaviour {

    static private List<AI.AIAgent> agents;
    static private List<GameObject> agentsObject;

	void Awake () {
        agents = new List<AI.AIAgent>();
        agentsObject = new List<GameObject>();
    }

    static public void registerAIAgent(AI.AIAgent go)
    {
        agents.Add(go);
        agentsObject.Add(go.gameObject);
    }

    static public List<GameObject> getActiveAgents()
    {
        return agentsObject;
    }

    static public List<AI.AIAgent> getActiveAIAgents()
    {
        return agents;
    }

    
}
