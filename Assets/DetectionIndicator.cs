using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionIndicator : MonoBehaviour {

    public GameObject[] players;

    private List<Transform> playerTransforms;
    private List<List<DetectionData>> trackers;

    private List<GameObject> arrowPool;
    private List<MeshRenderer> arrowRenderer;
    private int arrowCounter;

    private float prevHighest = 0f;

    [Header("Display Settings")]
    [Range(1.7f, 3f)]
    public float height;
    [Range(0.5f, 5f)]
    public float width;
    [Range(0.5f, 3f)]
    public float scaleModifier;
    public GameObject ArrowPrefab;
    //color of not offensive AI
    public Color BasicLow;
    public Color BasicHigh;
    public Color BasicDetected;
    public Color BasicTarget;
    //color of aggressive AI
    public Color AggressiveLow;
    public Color AggressiveHigh;
    public Color AggressiveDetected;
    public Color AggressiveTarget;

    void Start()
    {
        //Init wwise sounds for detection
        AkSoundEngine.SetRTPCValue("spotted", 0, Camera.main.gameObject);
        AkSoundEngine.PostEvent("spotting", Camera.main.gameObject);

        arrowCounter = 0;
        arrowPool = new List<GameObject>();
        arrowRenderer = new List<MeshRenderer>();

        for (int i = 0; i < 10; i++)
        {
            arrowPool.Add(Instantiate(ArrowPrefab));
            arrowRenderer.Add(arrowPool[i].GetComponentInChildren<MeshRenderer>());
        }

        playerTransforms = new List<Transform>();
        trackers = new List<List<DetectionData>>();

        //find the player transforms
        foreach (GameObject go in players)
            playerTransforms.Add(go.transform.Find("NotStatic").transform);
        //get the trackers list reference
        foreach (Transform t in playerTransforms)
            trackers.Add(AIAgentManager.getDetectData(t.gameObject));
    }
	
    public void UpdateTrackersList()
    {
        arrowCounter = 0;
        //loop through all players
        float highestDetectionLevel = 0;
        for (int i = 0; i < trackers.Count; i++)
        {
            trackers[i] = AIAgentManager.getDetectData(playerTransforms[i].gameObject);
            foreach (DetectionData data in trackers[i])
            {
                //update arrow
                placeArrowFromDataAtPosition(data, playerTransforms[i], false);
                if (data.detectRate > highestDetectionLevel) highestDetectionLevel = data.detectRate;
            }
        }
        if (prevHighest != 0 || highestDetectionLevel > 0 && highestDetectionLevel < 1)
        {
            AkSoundEngine.SetRTPCValue("spotted", highestDetectionLevel, Camera.main.gameObject);
        }

        if (highestDetectionLevel < 1 && prevHighest == 1)
        {
            AkSoundEngine.PostEvent("spotting", Camera.main.gameObject);
        }

        if (highestDetectionLevel == 1 && prevHighest !=1)
        {
            AkSoundEngine.PostEvent("spotted", Camera.main.gameObject);
        }
        prevHighest = highestDetectionLevel;
        //disabled the unused ones
        for (int i = arrowCounter; i < arrowPool.Count; i++)
        {
            arrowPool[i].SetActive(false);
        }
    }

    private void placeArrowFromDataAtPosition(DetectionData data, Transform transform, bool targeted)
    {
        //place the arrow above player head
        GameObject arrow = getArrow(transform);
        Renderer renderer = getArrowRenderer();
        arrow.transform.position = transform.position + Vector3.up * height;
        Vector3 agentPos = data.agent.transform.position;
        agentPos.y = arrow.transform.position.y;
        arrow.transform.localScale = (data.detectRate == 1 && targeted) ? Vector3.one *2 : Vector3.one * data.detectRate + Vector3.forward * data.detectRate;
        arrow.transform.localScale *= scaleModifier; 
        arrow.transform.LookAt(agentPos);
        arrow.transform.position += arrow.transform.forward.normalized * width;
        //set its color
        renderer.material.SetColor("_Color", getArrowColor(data.agent.type, data.detectRate, targeted));
    }

    private Color getArrowColor(AIType type, float detectionLevel, bool targeted)
    {
        bool isAgressive = type == AIType.Hunter;
        if (targeted)
        {
            if (isAgressive) return AggressiveTarget;
            else return BasicTarget;
        }
        
        if(detectionLevel < 1)
        {
            if (isAgressive)
                return Color.Lerp(AggressiveLow, AggressiveHigh, detectionLevel);
            else
                return Color.Lerp(BasicLow, BasicHigh, detectionLevel);
        }
        else
        {
            if (isAgressive)
                return AggressiveDetected;
            else
                return BasicDetected;
        }
    }

    private GameObject getArrow(Transform parent)
    {
        while (arrowCounter >= arrowPool.Count)
        {
            arrowPool.Add(Instantiate(ArrowPrefab));
            arrowRenderer.Add(arrowPool[arrowPool.Count-1].GetComponentInChildren<MeshRenderer>());
        }
        
        GameObject arrow = arrowPool[arrowCounter];
        arrow.transform.parent = parent;
        if(!arrow.activeInHierarchy)
            arrow.SetActive(true);

        ++arrowCounter;

        return arrow;
    }

    private MeshRenderer getArrowRenderer()
    {
        return arrowRenderer[arrowCounter - 1];
    }

}
