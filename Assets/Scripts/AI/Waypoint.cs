using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Waypoint : MonoBehaviour {

    [System.Serializable]
    public class Wpoint
    {
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
    public int current = 0;
    public Vector3 currentDestination;

    // Use this for initialization
    void Start() {
        currentDestination = this[current];
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
}
