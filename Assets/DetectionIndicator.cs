using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionIndicator : MonoBehaviour {

    public GameObject[] players;

    private List<Transform> playerTransforms;
    private List<GameObject> playerMeshGO;

    private List<List<DetectionData>> trackers;

    [Header("Display Settings")]
    [Range(1.7f, 3f)]
    public float height;
    [Range(0.5f, 5f)]
    public float width;
    public GameObject ArrowPrefab;
    //color of not offensive AI
    public Color BasicLow;
    public Color BasicHigh;
    //color of aggressive AI
    public Color AggressiveLow;
    public Color AggressiveHigh;

    void Start() {
        playerTransforms = new List<Transform>();
        playerMeshGO = new List<GameObject>();
        trackers = new List<List<DetectionData>>();

        //find the player transforms
        foreach (GameObject go in players)
            playerTransforms.Add(go.transform.Find("NotStatic").transform);
        //find the player mesh gameObject owners
        foreach (Transform t in playerTransforms)
            playerMeshGO.Add(t.Find("cannibal").gameObject);
        //get the trackers list reference
        foreach (GameObject go in playerMeshGO)
            trackers.Add(AIAgentManager.getDetectData(go));
    }
	
    public void UpdateTrackersList()
    {
        //loop through all players
        for (int i = 0; i < trackers.Count; i++)
        {
            foreach(DetectionData data in trackers[i])
            {
                //update arrow
                placeArrowFromDataAtPosition(data, playerTransforms[i].position);
            }
        }
    }

    private void placeArrowFromDataAtPosition(DetectionData data, Vector3 position)
    {
        //place the arrow above player head
        GameObject arrow = getArrow();
        arrow.transform.position = position + Vector3.up * height;
        Vector3 agentPos = data.agent.transform.position;
        agentPos.y = arrow.transform.position.y;
        arrow.transform.LookAt(agentPos);
        arrow.transform.position += arrow.transform.forward.normalized * width;
        //set its color
        arrow.GetComponentInChildren<Material>().SetColor("_Color", getArrowColor(data.agent.type, data.detectRate));
    }

    private Color getArrowColor(AIType type, float detectionLevel)
    {
        if (type == AIType.Hunter)
            return Color.Lerp(AggressiveLow, AggressiveHigh, detectionLevel);
        else
            return Color.Lerp(BasicLow, BasicHigh, detectionLevel);
    }

    private GameObject getArrow()
    {
        return new GameObject();
    }
	
}
