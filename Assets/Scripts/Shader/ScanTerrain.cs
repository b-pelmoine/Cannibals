using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScanTerrain : MonoBehaviour
{
    public Transform ScannerOrigin;
    public Material EffectMaterial;
    public float ScanDistance;

    private float speed;
    [Range(5f, 20f)]
    public float OnePlayerSpeed;
    [Range(20f, 50f)]
    public float TwoPlayersSpeed;

    private Camera _camera;
    public OffscreenIndicator indicator;

    private List<int> RegisteredUIDs;

    // Demo Code
    bool _scanning;
    float elapsed = 0f;
    int prev_ActiveUsers = 0;
    GameObject[] _scannables;

    void Start()
    {
        RegisteredUIDs = new List<int>();
        _scannables = AIAgentManager.getActiveAgents().ToArray();
    }

    public void StartScan(Vector3 src)
    {
        _scannables = AIAgentManager.getActiveAgents().ToArray();
        RegisteredUIDs.Clear();
        _scanning = true;
        ScanDistance = 0;
        ScannerOrigin.position = src;
        speed = OnePlayerSpeed;
    }

    public void UpdateScan(int activeUsers)
    {
        if(prev_ActiveUsers != activeUsers)
        {
            if (activeUsers == 1 && prev_ActiveUsers == 0)
            {
                AkSoundEngine.PostEvent("sense_layer1", ScannerOrigin.gameObject);
            }
            else
            {
                if (activeUsers == 2)
                {
                    AkSoundEngine.PostEvent("sense_layer2", ScannerOrigin.gameObject);
                }
                else
                {
                    if(activeUsers == 1 && prev_ActiveUsers == 2)
                        AkSoundEngine.PostEvent("sense_layer2_end", ScannerOrigin.gameObject);

                }
            }   
        }
        prev_ActiveUsers = activeUsers;
        switch (activeUsers)
        {
            case 1: speed = OnePlayerSpeed; break;
            case 2: speed = TwoPlayersSpeed; break;
            default: stopScan();break;
        }
        
    }

    void stopScan()
    {
        _scanning = false;
        elapsed = 0;
    }

    void Update()
    {
        if (_scanning)
        {
            ScanDistance += Time.deltaTime * speed;
            foreach (GameObject s in _scannables)
            {
                if (Vector3.Distance(ScannerOrigin.position, s.transform.position) <= ScanDistance)
                {
                    int sUID = s.GetInstanceID();
                    bool exit = false;
                    foreach (int id in RegisteredUIDs) { if (id == sUID) exit = true; }
                    if (!exit)
                    {
                        AI.AIAgent agent = s.GetComponent<AI.AIAgent>();
                        if (agent)
                        {
                            if(agent.type != AIType.UNKNOWN)
                                indicator.AddAgentIndicator(s, agent.type);
                        }
                        RegisteredUIDs.Add(sUID);
                    }
                }
                    
            }
        }
        else
        {
            elapsed += Time.deltaTime;
            if (3f - elapsed > 0)
                ScanDistance += Time.deltaTime * OnePlayerSpeed/2;
            else
                ScanDistance = 0;
        }
    }
    // End Demo Code

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
        EffectMaterial.shader = Shader.Find("Hidden/ScannerEffect");
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        EffectMaterial.SetVector("_WorldSpaceScannerPos", ScannerOrigin.position);
        EffectMaterial.SetFloat("_ScanDistance", ScanDistance);
        RaycastCornerBlit(src, dst, EffectMaterial);
    }

    void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = _camera.farClipPlane;
        float camFov = _camera.fieldOfView;
        float camAspect = _camera.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = _camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = _camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (_camera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (_camera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (_camera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (_camera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
