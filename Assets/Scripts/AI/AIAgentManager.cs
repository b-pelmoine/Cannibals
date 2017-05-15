using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DetectionData
{
    public AI.AIAgent agent;
    public float detectRate = 0;
}

public class AIAgentManager : MonoBehaviour {

    static public List<GameObject> agents;
    
    static Dictionary<GameObject, List<DetectionData>> detected = new Dictionary<GameObject, List<DetectionData>>();

	void Awake () {
        agents = new List<GameObject>();
    }

    static public void registerAIAgent(GameObject go)
    {
        if(go.GetComponent<AI.AIAgent>())
            agents.Add(go);
    }

    static public List<GameObject> getActiveAgents()
    {
        return agents;
    }

    static public void addDetectData(GameObject target, DetectionData data)
    {
        if (!detected.ContainsKey(target))
        {
            detected.Add(target, new List<DetectionData>());
        }
        detected[target].Add(data);
    }

    static public void deleteDetectData(GameObject target, DetectionData data)
    {
        detected[target].Remove(data);
    }

    static public List<DetectionData> getDetectData(GameObject target)
    {
        if (!detected.ContainsKey(target))
        {
            detected.Add(target, new List<DetectionData>());
        }
        return detected[target];
    }
    
}
