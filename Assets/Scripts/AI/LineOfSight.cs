using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SightInfo
{
    public GameObject target;
    public float time = 0;

    public SightInfo(GameObject info)
    {
        target = info;
    }
}

public class LineOfSight : MonoBehaviour {
    public new Camera camera;
    public Shader shader;
    public static RenderTexture texture;
    static Texture2D tex2D;
    static List<GameObject> detected_objects = new List<GameObject>();
    static Dictionary<Collider, GameObject> colliders = new Dictionary<Collider, GameObject>();
    public List<SightInfo> sighted;
    static List<int> detect_rate= new List<int>();
    bool updated = false;
    public bool active = true;
    
    public enum SightType
    {
        Camera,
        SimpleView,
        AllAround
    }
    public SightType type = SightType.Camera;
    public float radius = 3;
    public float detectTime = 3;

    private float lastTime = -1;

    void Awake()
    {
        if (texture == null)
            texture = new RenderTexture(128, 128, 32, RenderTextureFormat.ARGB32);
        if (tex2D == null)
            tex2D = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        lastTime = Time.time;
    }

	void Start () {
        if (camera == null && type == SightType.Camera)
        {
            camera = GetComponentInChildren<Camera>();
            if (camera == null)
                camera = gameObject.AddComponent<Camera>();
        }
        if (camera != null)
        {
            camera.enabled = false;
            camera.targetTexture = texture;
            camera.SetReplacementShader(shader, "");
        }
        sighted = new List<SightInfo>();
        
        LineOfSightManager.Register(this);
    }

    public float getSeeDistance()
    {
        if(type == SightType.Camera)
        {
            return camera.farClipPlane;
        }
        else
        {
            return radius;
        }
    }

    public float getDetectRate(SightInfo si)
    {
        float detectRate = Mathf.Clamp((((si.time+(Time.time-lastTime)) / detectTime) *Mathf.Pow( (getSeeDistance()), 2))
                / (si.target.transform.position - transform.position).sqrMagnitude, 0, 1);
        return detectRate;
    }

    public static void Register(GameObject obj, int detect=30, Color? color = null)
    {
        detected_objects.Add(obj);
        detect_rate.Add(detect);
        List<Renderer> renderers = new List<Renderer>();
        obj.GetComponentsInChildren<Renderer>(renderers);
        if (renderers.Count <= 0)
            Debug.LogError("LineOfSight.cs: No renderers found on " + obj);
        foreach (Renderer rend in renderers)
            foreach(Material mat in rend.materials)
                if(color.HasValue) mat.SetColor("_LoSColor", color.Value);
                else mat.SetColor("_LoSColor", new Color(detected_objects.Count / 255f, 0, 0));
        Collider col = obj.GetComponentInChildren<Collider>();
        if(col!=null)
            colliders.Add(col, obj);
    }

    public static void Register(GameObject obj, MeshRenderer mesh, int detect = 30)
    {
        detected_objects.Add(obj);
        detect_rate.Add(detect);
        foreach (Material mat in mesh.materials)
            mat.SetColor("_LoSColor", new Color(detected_objects.Count / 255f, 0, 0));
    }

    public static void UnRegister(GameObject obj)
    {
        int pos = detected_objects.FindIndex(x => x==obj);
        if(pos>=0 && pos < detected_objects.Count)
        {
            detected_objects.RemoveAt(pos);
            detect_rate.RemoveAt(pos);
            var keys = colliders.Where(x => x.Value == obj).ToList();
            foreach (var k in keys)
                colliders.Remove(k.Key);
        }
    }

    public bool Analyse()
    {
        if (!gameObject.activeInHierarchy) return false;
        sighted.RemoveAll(x => x.time < 0);
        foreach (SightInfo si in sighted)
            si.time -= Time.time - lastTime;
        if (type == SightType.Camera && active) AnalyseSight();
        else if (type == SightType.Camera) AnalyseSimple();
        AnalyseAllAround();
        updated = true;
        
        lastTime = Time.time;
        return sighted.Count != 0;
    }

    public void AnalyseSight()
    {
        if (camera == null) return;
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
        for (int i = 0; i < detected_objects.Count; i++)
        {
            
            if (pixnum[i] > detect_rate[i])
            {
                
                SightInfo si = sighted.Find(x => x.target == detected_objects[i]);
                if (si != null)
                {
                    si.time = Mathf.Clamp(si.time + (Time.time - lastTime)*2, -5.0f, detectTime);
                }
                else// if((si.target.transform.position-transform.position).sqrMagnitude < 0.95*Mathf.Pow(getSeeDistance(), 2))
                {
                    
                    sighted.Add(new SightInfo(detected_objects[i]));
                }
            }
        }
        RenderTexture.active = null;
    }

    public void AnalyseSimple()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, camera.nearClipPlane + camera.farClipPlane);
        foreach(Collider c in cols)
        {
            if (colliders.ContainsKey(c) && Mathf.Abs(Vector3.Angle(transform.forward, c.transform.position-transform.position))<45)
            {
                SightInfo si = sighted.Find(x => x.target == colliders[c]);
                if (si != null)
                    si.time = Mathf.Clamp(si.time + (Time.time - lastTime) * 2, -5.0f, detectTime);
                else
                    sighted.Add(new SightInfo(colliders[c]));
            }
        }
    }

    public void AnalyseAllAround()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        List<SightInfo> bushes = sighted.FindAll(x => x.target.GetComponent<Bush>() != null);
        foreach(Collider c in cols)
        {
            if (colliders.ContainsKey(c))
            {
                if ((1<<c.gameObject.layer & LayerMask.GetMask("Bush"))==0 && bushes.FindAll(x => (x.target.transform.position-c.transform.position).sqrMagnitude<2).Count!=0)
                {
                    continue;
                }
                SightInfo si = sighted.Find(x => x.target == colliders[c]);
                
                if (si != null)
                    si.time = Mathf.Clamp(si.time + (Time.time - lastTime) * 2, -5.0f, detectTime);
                else
                    sighted.Add(new SightInfo(colliders[c]));
            }
        }
    }

    public void Rendering()
    {
        if(gameObject.activeInHierarchy && camera!=null && type==SightType.Camera && active)
            camera.Render();
    }

    public SightInfo FindNearest(Predicate<SightInfo> p)
    {
        SightInfo result = null;
        foreach(SightInfo si in sighted)
        {
            if (p(si) && (result == null || (si.target.transform.position - transform.position).sqrMagnitude < (result.target.transform.position).sqrMagnitude))
                result = si;
        }
        return result;
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
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
#endif
}
