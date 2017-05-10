using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public OffscreenIndicator indicator;
    public ScanTerrain scanner;

    public bool huntSensActive = false;
    private float elapsedTime;
    [Range(0.2f, 3f)]
    public float transitionDuration;
    [Range(0.1f, 1f)]
    public float HuntSenseFinalIntensity;

    private Dictionary<string, bool> cannibalsHuntSenseState;

    // Use this for initialization
    void Start()
    {
        elapsedTime = 50f;
        cannibalsHuntSenseState = new Dictionary<string, bool>();
    }

    // Update is called once per frame
    void Update()
    {
        //update UI for hunt sense
        UpdateHuntSense();
    }

    public void triggerHuntSense(string cannibalID, bool state)
    {
        cannibalsHuntSenseState[cannibalID] = state;
        huntSensActive = false;
        //check if there is at least one cannibal using the huntersense
        int activeCount = 0;
        foreach (bool state_t in cannibalsHuntSenseState.Values)
        {
            if (state_t == true)
            {
                huntSensActive = true;
                ++activeCount;
            }
        }
        elapsedTime = 0;
        indicator.triggerAgentIndicator(huntSensActive);
        if(huntSensActive)
        {
            RewiredInput[] inputs = GameObject.FindObjectsOfType<RewiredInput>();
            Vector3 pos = Vector3.zero;
            foreach (RewiredInput input in inputs) if (input.playerInputID == cannibalID) pos = input.transform.position;
            scanner.StartScan(pos);
        }
        //casually update scanner data
        scanner.UpdateScan(activeCount);
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
            Shader.SetGlobalFloat("_HuntSenseIntensity", effectIntensity);
        }
        else
        {
            if (elapsedTime < transitionDuration)
            {
                float effectIntensity = ((transitionDuration - elapsedTime) / transitionDuration) * HuntSenseFinalIntensity;
                elapsedTime += Time.deltaTime;
                if (elapsedTime > transitionDuration) elapsedTime = transitionDuration;
                Shader.SetGlobalFloat("_HuntSenseIntensity", effectIntensity);
            }
        }
    }
}
