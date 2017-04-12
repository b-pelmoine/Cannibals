using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionShaderManager : MonoBehaviour {
    public List<Transform> objs;
    public List<Vector4> objects;
    public float Width = 1;
    //public Vector4 cam;
    private static OcclusionShaderManager _instance;
    // Use this for initialization
    void Start () {
        _instance = this;
        objects.Clear();
        foreach (Transform trans in objs)
            objects.Add(trans.position);
    }

    public static OcclusionShaderManager Instance()
    {
        return _instance;
    }

    // Update is called once per frame
    void Update () {

        objects.Clear();
        foreach (Transform trans in objs)
            objects.Add(trans.position);

    }
}
