using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Waypoint : MonoBehaviour {

    [System.Serializable]
    public class Wpoint
    {
        public string tag;
        public Vector3 position;
        public List<int> links;

        public Wpoint(Vector3 point)
        {
            position = point;
            links = new List<int>();
        }

        public int this[int key]
        {
            get
            {
                return links[key];
            }
            set
            {
                links[key] = value;
            }
        }
    }
    public List<Wpoint> points = new List<Wpoint>();
    private int current = 0;
    public Vector3 currentDestination;

    // Use this for initialization
    void Start() {
        currentDestination = this[current];
    }

    public void Reset()
    {
        current = 0;
    }

    public void Add(Vector3 point)
    {
        points.Add(new Wpoint(point));
    }

    public void AddLink(int src, int dst)
    {
        if (src < 0 || src >= points.Count || dst < 0 || dst > points.Count || points[src].links.Contains(dst))
            return;
        points[src].links.Add(dst);
    }

    public void Delete(int point)
    {
        points.RemoveAt(point);
        for (int i = 0; i < points.Count; i++)
        {
            points[i].links.Remove(point);
            for (int j = 0; j < points[i].links.Count; j++)
                if (points[i].links[j] > point)
                    points[i].links[j]--;
        }
    }

    public int getNearest(Vector3 position)
    {
        int nearest = -1;
        for(int i = 0; i < points.Count; i++)
        {
            if (nearest == -1 || (position - this[i]).sqrMagnitude < (position - this[nearest]).sqrMagnitude)
                nearest = i;
        }
        return nearest;
    }

    public Vector3 getCurrentDestination()
    {
        if(points.Count <= 0)
        {
            Debug.LogError("No waypoint placed");
            return new Vector3();
        }
        return this[current];
    }

    public void nextWaypoint()
    {
        current = points[current][0];
        currentDestination = this[current];
    }

    public int getNext(int id)
    {
        return points[id][0];
    }

    public bool Next()
    {
        current = (current + 1) % points.Count;
        if (current == 0)
            return true;
        return false;
    }

    public Vector3 this[int key]
    {
        get
        {
            return points[key].position;
        }
        set
        {
            points[key].position=value;
        }
    }
    
    void OnDrawGizmos()
    {
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i - 1].position + Vector3.up, points[i].position + Vector3.up);
        }
        Gizmos.DrawLine(points[0].position + Vector3.up, points[points.Count - 1].position + Vector3.up);
    }
}
