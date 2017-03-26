using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentManager : MonoBehaviour {

    public GameObject testAgentPrefab;
    public Material agentMaterial;
    public Terrain walkable;
    [Range (1.0f,100.0f)]
    public float framerate = 30.0f;

    private List<GameObject> agents;
    private List<LineOfSight> agents_sight;
    int turn = 0;

	void Start () {
        agents = new List<GameObject>();
        agents_sight = new List<LineOfSight>();
		for(int i=0; i<5; i++)
        {
            GameObject instance = Instantiate(testAgentPrefab, transform);

            AIAgentPriorityTest script = instance.GetComponent<AIAgentPriorityTest>();
            LevelOfImportance level = instance.GetComponent<LevelOfImportance>();
            level.setLevel(Mathf.RoundToInt(Random.Range(1,10)));
            script.agent = instance.GetComponent<NavMeshAgent>();
            script.walkable = walkable;
            script.destination = Vector3.zero;

            agents.Add(instance);
            agents_sight.Add(instance.GetComponentInChildren<LineOfSight>());
        }
        StartCoroutine(Detect());
    }

    public IEnumerator Detect()
    {
        while (true)
        {
            walkable.drawTreesAndFoliage = false;
            agents_sight[turn].Rendering();
            walkable.drawTreesAndFoliage = true;
            yield return new WaitForSeconds(1/framerate);
            agents_sight[turn].Analyse();
            turn = (turn + 1) % agents.Count;
        }
    }
}
