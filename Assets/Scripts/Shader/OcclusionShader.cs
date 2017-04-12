using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionShader : MonoBehaviour {
    Material mat;
	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
        
	}
	
	// Update is called once per frame
	void Update () {
        mat.SetFloat("_Distance", OcclusionShaderManager.Instance().Width);
        mat.SetVectorArray("_Objects", OcclusionShaderManager.Instance().objects);
        mat.SetInt("_ObjectsLength", OcclusionShaderManager.Instance().objects.Count);
    }
}
