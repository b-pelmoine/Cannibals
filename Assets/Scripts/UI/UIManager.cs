using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Hunt Sense")]
    public OffscreenIndicator indicator;
    public ScanTerrain scanner;
    
    public bool huntSensActive = false;
    private float elapsedTime;
    [Range(0.2f, 3f)]
    public float transitionDuration;
    [Range(0.5f, 5f)]
    public float DecaytransitionDuration;
    [Range(0.1f, 1f)]
    public float HuntSenseFinalIntensity;
    [Space(30)]
    private Dictionary<string, bool> cannibalsHuntSenseState;

    // Use this for initialization
    void Start()
    {
        elapsedTime = 50f;
        cannibalsHuntSenseState = new Dictionary<string, bool>();
        Shader.SetGlobalFloat("_HuntSenseIntensity", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //update UI for hunt sense
        UpdateHuntSense();
    }

    public void triggerHuntSense(string cannibalID, bool state)
    {
        bool huntSense_t = huntSensActive;
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
        if(huntSensActive && huntSense_t != huntSensActive)
        {
            if (elapsedTime > DecaytransitionDuration) elapsedTime = DecaytransitionDuration;
            elapsedTime = (elapsedTime < DecaytransitionDuration) ? elapsedTime * (transitionDuration / DecaytransitionDuration) : 0;
            indicator.triggerAgentIndicator(huntSensActive);
            RewiredInput[] inputs = GameObject.FindObjectsOfType<RewiredInput>();
            Vector3 pos = Vector3.zero;
            foreach (RewiredInput input in inputs) if (input.playerInputID + input.number == cannibalID) pos = input.transform.Find("NotStatic").transform.position;
            scanner.StartScan(pos);
        }
        else
        {
            indicator.triggerAgentIndicator(huntSensActive);
        }
        if(!huntSensActive)
        {
            elapsedTime = (elapsedTime >= transitionDuration) ? 0 : DecaytransitionDuration - (elapsedTime * (DecaytransitionDuration / transitionDuration));
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
            if (elapsedTime < DecaytransitionDuration)
            {
                float effectIntensity = ((DecaytransitionDuration - elapsedTime) / DecaytransitionDuration) * HuntSenseFinalIntensity;
                elapsedTime += Time.deltaTime;
                if (elapsedTime > DecaytransitionDuration) elapsedTime = DecaytransitionDuration;
                Shader.SetGlobalFloat("_HuntSenseIntensity", effectIntensity);
            }
        }
    }
}
