using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineOfSight : MonoBehaviour {
    public new Camera camera;
    public Shader shader;
    public static RenderTexture texture;
    static Texture2D tex2D;
    static List<GameObject> detected_objects = new List<GameObject>();
    public List<GameObject> sighted;
    static List<int> detect_rate= new List<int>();
    bool updated = false;
    public bool active = true;
    

    void Awake()
    {
        detected_objects = new List<GameObject>();
        detect_rate = new List<int>();
        if (texture == null)
            texture = new RenderTexture(128, 128, 32, RenderTextureFormat.ARGB32);
        if (tex2D == null)
            tex2D = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
    }

	void Start () {
        if (camera == null)
        {
            camera = GetComponentInChildren<Camera>();
            if (camera == null)
                camera = gameObject.AddComponent<Camera>();
        }
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
        if (renderers.Count <= 0)
            Debug.LogError("LineOfSight.cs: No renderers found on " + obj);
        foreach (Renderer rend in renderers)
            foreach(Material mat in rend.materials)
                mat.SetColor("_LoSColor", new Color(detected_objects.Count / 255f, 0, 0));
    }

    public static void Register(GameObject obj, MeshRenderer mesh, int detect = 30)
    {
        detected_objects.Add(obj);
        detect_rate.Add(detect);
        foreach (Material mat in mesh.materials)
            mat.SetColor("_LoSColor", new Color(detected_objects.Count / 255f, 0, 0));
    }

    public bool Analyse()
    {
        if (active) AnalyseSight();
        else AnalyseSimple();
        updated = true;
        return sighted.Count != 0;
    }

    public void AnalyseSight()
    {
        RenderTexture.active = texture;
        tex2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0, false);
        Color[] pixels = tex2D.GetPixels();
        int[] pixnum = new int[detected_objects.Count];
        int red;
        for (int i = 0; i < pixels.Length; i++)
        {
            red = (int)(pixels[i].r * 255) - 1;
            if (red >= 0 && red < detected_objects.Count)
                pixnum[red]++;
        }
        sighted.Clear();
        for (int i = 0; i < detected_objects.Count; i++)
        {
            if (pixnum[i] > detect_rate[i])
            {
                sighted.Add(detected_objects[i]);
            }
        }
        RenderTexture.active = null;
    }

    public void AnalyseSimple()
    {
        sighted.Clear();
        Collider[] cols = Physics.OverlapSphere(transform.position, camera.nearClipPlane + camera.farClipPlane);
        foreach(Collider c in cols)
        {
            if (detected_objects.Contains(c.gameObject) && Mathf.Abs(Vector3.Angle(transform.forward, c.transform.position-transform.position))<45)
            {
                sighted.Add(c.gameObject);
            }
        }
    }

    public void Rendering()
    {
        if(active)
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
