using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightManager : MonoBehaviour {
    public Terrain walkable;
    [Range(1.0f, 100.0f)]
    public float framerate = 30.0f;
    static private List<LineOfSight> agents_sight;
    int turn = 0;
    public List<GameObject> players;
    public float render_distance = 30;

    // Use this for initialization
    void Awake () {
        agents_sight = new List<LineOfSight>();
        StartCoroutine(Detect());
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
            agents_sight[turn].active = true;
            foreach(GameObject p in players)
            {
                if ((agents_sight[turn].transform.position - p.transform.position).sqrMagnitude > render_distance * render_distance)
                {
                    agents_sight[turn].active = false;
                    break;
                }
            }
            walkable.drawTreesAndFoliage = false;
            agents_sight[turn].Rendering();
            walkable.drawTreesAndFoliage = true;
            yield return new WaitForSeconds(1 / (framerate*agents_sight.Count));
            agents_sight[turn].Analyse();
            turn = (turn + 1) % agents_sight.Count;
        }
    }
}
