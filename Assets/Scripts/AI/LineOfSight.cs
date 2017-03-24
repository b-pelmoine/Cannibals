using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LineOfSight : MonoBehaviour {
    new Camera camera;
    public Shader shader;
    RenderTexture texture;
    public float framerate = 1.0f;
    static Texture2D tex2D;
    static List<GameObject> detected_objects;
    public List<GameObject> sighted;

	// Use this for initialization
	void Awake () {
        camera = GetComponent<Camera>();
        camera.enabled = false;
        texture = camera.targetTexture;
        if(tex2D==null)
            tex2D = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        camera.SetReplacementShader(shader, "RenderType");
        sighted = new List<GameObject>();
        if (detected_objects == null)
            detected_objects = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public static void Register(GameObject obj)
    {
        detected_objects.Add(obj);
        obj.GetComponent<Renderer>().material.SetColor("LoSColor", new Color(detected_objects.Count,0,0));
    }

    public bool Analyse()
    {
        RenderTexture.active = texture;
        tex2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0, false);
        Color[] pixels = tex2D.GetPixels();
        int[] pixnum = new int[detected_objects.Count];
        int red;
        for (int i = 0; i < pixels.Length; i++)
        {
            red = (int)(pixels[i].r * 255) - 1;
            if ( red >= 0 && red < detected_objects.Count)
                pixnum[red]++;
        }
        sighted.Clear();
        for(int i = 0; i < detected_objects.Count;i++)
        {
            if (pixnum[i] > 30)
            {
                sighted.Add(detected_objects[i]);
                Debug.Log(detected_objects[i] + " detected");
            }
        }
        RenderTexture.active = null;
        return sighted.Count != 0;
    }

    public void Rendering()
    {
        camera.Render();
    }
}
