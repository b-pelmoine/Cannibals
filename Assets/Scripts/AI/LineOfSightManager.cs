using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightManager : MonoBehaviour {
    public Terrain walkable;
    [Range(1.0f, 100.0f)]
    public float framerate = 30.0f;
    static private List<LineOfSight> agents_sight;
    int turn = 0;

    // Use this for initialization
    void Awake () {
        agents_sight = new List<LineOfSight>();
        StartCoroutine(Detect());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void Register(LineOfSight agent)
    {
        agents_sight.Add(agent);
    }

    public IEnumerator Detect()
    {
        while (agents_sight.Count == 0)
            yield return new WaitForSeconds(1/framerate);
        while (true)
        {
            walkable.drawTreesAndFoliage = false;
            agents_sight[turn].Rendering();
            walkable.drawTreesAndFoliage = true;
            yield return new WaitForSeconds(1 / (framerate*agents_sight.Count));
            agents_sight[turn].Analyse();
            turn = (turn + 1) % agents_sight.Count;
        }
    }
}
