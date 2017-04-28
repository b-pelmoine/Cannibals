using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public OffscreenIndicator indicator;
    public AlterMaterial huntSenseTerrain;
    public AIAgentManager IAmanager;

    private bool huntSensActive;

	// Use this for initialization
	void Start () {
        huntSenseTerrain.agentGO = IAmanager.getActiveAgents();
    }
	
	// Update is called once per frame
	void Update () {
        if(huntSensActive)
        {
            indicator.AIAgents = IAmanager.getActiveAgents().ToArray();
        }
	}

    public void triggerHuntSense(bool state)
    {
        huntSensActive = state;
        huntSenseTerrain.effectActive = state;
        indicator.triggerAgentIndicator(state);
    }
}
