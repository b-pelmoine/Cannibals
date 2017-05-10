using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ScanTerrain : MonoBehaviour
{
    public Transform ScannerOrigin;
    public Material EffectMaterial;
    public float ScanDistance;

    private Camera _camera;
    private FollowCam camMover;
    public OffscreenIndicator indicator;

    private List<int> RegisteredUIDs;

    // Demo Code
    bool _scanning;
    GameObject[] _scannables;

    void Start()
    {
        _scannables = AIAgentManager.getActiveAgents().ToArray();
        camMover = gameObject.GetComponent<FollowCam>();
        _scanning = true; ScanDistance = 0;
    }

    void Update()
    {
        if (_scanning)
        {
            ScanDistance += Time.deltaTime * 25;
            foreach (GameObject s in _scannables)
            {
                if (Vector3.Distance(ScannerOrigin.position, s.transform.position) <= ScanDistance)
                {
                    int sUID = s.GetInstanceID();
                    bool exit = false;
                    foreach (int id in RegisteredUIDs) { if (id == sUID) exit = true; }
                    if (!exit)
                    {
                        indicator.AddAgentIndicator(s, s.GetComponent<AI.AIAgent>().type);
                        RegisteredUIDs.Add(sUID);
                    }
                }
                    
            }
        }

        if (ScanDistance > 80)
        {
                _scanning = true;
                ScanDistance = 0;
                ScannerOrigin.position = camMover.getPlayersBarycenter();
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
