﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineOfSight : MonoBehaviour {
    new Camera camera;
    public Shader shader;
    public static RenderTexture texture;
    static Texture2D tex2D;
    static List<GameObject> detected_objects = new List<GameObject>();
    public List<GameObject> sighted;
    static List<int> detect_rate= new List<int>();
    bool updated = false;

    void Awake()
    {
        if (texture == null)
            texture = new RenderTexture(128, 128, 32, RenderTextureFormat.ARGB32);
        if (tex2D == null)
            tex2D = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
    }

	void Start () {
        camera = GetComponentInChildren<Camera>();
        if (camera == null)
            camera = gameObject.AddComponent<Camera>();
        camera.enabled = false;
        camera.targetTexture = texture;
        
        camera.SetReplacementShader(shader, "RenderType");
        sighted = new List<GameObject>();
        
        LineOfSightManager.Register(this);
    }

    public static void Register(GameObject obj, int detect=30)
    {
        detected_objects.Add(obj);
        detect_rate.Add(detect);
        List<Renderer> renderers = new List<Renderer>();
        obj.GetComponentsInChildren<Renderer>(renderers);
        foreach (Renderer rend in renderers)
            rend.material.SetColor("_LoSColor", new Color(detected_objects.Count / 255f, 0, 0));
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
            if (pixnum[i] > detect_rate[i])
            {
                sighted.Add(detected_objects[i]);
            }
        }
        RenderTexture.active = null;
        updated = true;
        return sighted.Count != 0;
    }

    public void Rendering()
    {
        camera.Render();
    }

    public bool Updated
    {
        get
        {
            if (updated)
            {
                updated = false;
                return true;
            }
            return false;
        }
    }
}
