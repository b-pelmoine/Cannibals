using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public OffscreenIndicator indicator;
    public AlterMaterial huntSenseTerrain;
    public AIAgentManager IAmanager;

    public bool huntSensActive = false;
    private float elapsedTime;
    [Range(0.2f, 3f)]
    public float transitionDuration;
    [Range(0.1f, 1f)]
    public float HuntSenseFinalIntensity;
    [Range(2f, 5f)]
    public float agentSpreadAreaMax;

    private Dictionary<string, bool> cannibalsHuntSenseState;

	// Use this for initialization
	void Start () {
        huntSenseTerrain.agentGO = IAmanager.getActiveAgents();
        huntSenseTerrain.effectArea = 0;
        elapsedTime = 50f;
        cannibalsHuntSenseState = new Dictionary<string, bool>();
    }
	
	// Update is called once per frame
	void Update () {
        //update UI for hunt sense
        UpdateHuntSense();
	}

    public void triggerHuntSense(string cannibalID, bool state)
    {
        cannibalsHuntSenseState[cannibalID] = state;
        huntSensActive = false;
        //check if there is at least one cannibal using the huntersense
        foreach(bool state_t in cannibalsHuntSenseState.Values)
        {
            huntSensActive = huntSensActive || state_t;
        }
        elapsedTime = 0;
        indicator.triggerAgentIndicator(huntSensActive);
    }

    private void UpdateHuntSense()
    {
        if (huntSensActive)
        {
            float effectIntensity;
            if (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > transitionDuration) elapsedTime = transitionDuration;
                effectIntensity = (elapsedTime / transitionDuration) * HuntSenseFinalIntensity;
            }
            else
            {
                effectIntensity = HuntSenseFinalIntensity;
            }
            //update the array of AIAgents removing the ones that are dead
            GameObject[] activeAgents = IAmanager.getActiveAgents().ToArray();
            foreach(GameObject agent in activeAgents)
            {
                agent.GetComponent<Renderer>().material.SetFloat("_Intensity", effectIntensity);
            }
            huntSenseTerrain.effectArea = agentSpreadAreaMax * effectIntensity * 1/HuntSenseFinalIntensity;
            //if the list have changed
            if(activeAgents.Length != indicator.AIAgents.Length)
                indicator.AIAgents = activeAgents;
                
        }
        else
        {
            if (elapsedTime < transitionDuration)
            {
                float effectIntensity = ((transitionDuration - elapsedTime) / transitionDuration) * HuntSenseFinalIntensity;
                elapsedTime += Time.deltaTime;
                if (elapsedTime > transitionDuration) elapsedTime = transitionDuration;
                GameObject[] activeAgents = IAmanager.getActiveAgents().ToArray();
                foreach (GameObject agent in activeAgents)
                {
                    agent.GetComponent<Renderer>().material.SetFloat("_Intensity", effectIntensity);
                }
                huntSenseTerrain.effectArea = agentSpreadAreaMax * effectIntensity * 1 / HuntSenseFinalIntensity;
            }
        }
    }
}
