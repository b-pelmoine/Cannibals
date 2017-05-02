using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public OffscreenIndicator indicator;
    public AlterMaterial huntSenseTerrain;


    private bool huntSensActive;
    private float elapsedTime;
    [Range(0.2f, 3f)]
    public float transitionDuration;
    [Range(0.1f, 1f)]
    public float HuntSenseFinalIntensity;

	// Use this for initialization
	void Start () {
        huntSenseTerrain.agentGO = AIAgentManager.getActiveAgents();
    }
	
	// Update is called once per frame
	void Update () {
        //update UI for hunt sense
        UpdateHuntSense();
	}

    public void triggerHuntSense(bool state)
    {
        elapsedTime = 0;
        huntSensActive = state;
        huntSenseTerrain.effectActive = state;
        indicator.triggerAgentIndicator(state);
    }

    private void UpdateHuntSense()
    {
        if (huntSensActive)
        {
            if (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > transitionDuration) elapsedTime = transitionDuration;
                Shader.SetGlobalFloat("_Intensity", (elapsedTime / transitionDuration) * HuntSenseFinalIntensity);
            }
            else
            {
                Shader.SetGlobalFloat("_Intensity", HuntSenseFinalIntensity);
            }
            //update the array of AIAgents removing the ones that are dead
            indicator.AIAgents = AIAgentManager.getActiveAgents().ToArray();
        }
        else
        {
            if (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > transitionDuration) elapsedTime = transitionDuration;
                Shader.SetGlobalFloat("_Intensity", ((transitionDuration - elapsedTime) / transitionDuration) * HuntSenseFinalIntensity);
            }
        }
    }
}
