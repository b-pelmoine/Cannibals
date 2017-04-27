using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAmbiance : MonoBehaviour {

    [SerializeField]
    new Transform transform;

   /* [SerializeField]
    ForestAmbianceSound[] treesRepresentation;

    [SerializeField]
    float range = 10f;

    [SerializeField]
    LayerMask treeLayerMask; */

    [SerializeField]
    string forestAmbianceEvent = "ambiance_forest";

    public Terrain terrain;

 /*   List<ForestAmbianceSound> availablesTreesRepresentation = new List<ForestAmbianceSound>();

    Collider[] nearTreeColliders; */

#if UNITY_EDITOR
   // public bool debug = true;
#endif


    void Start()
    {
        TerrainData terrainData = terrain.terrainData;

        for(int i=0; i < terrainData.treeInstances.Length; i++)
        {
            GameObject go = new GameObject("AkPostForestAmbiance");
            go.transform.SetParent(transform);
            go.transform.position = terrainData.treeInstances[0].position;
            AkSoundEngine.PostEvent(forestAmbianceEvent, go);
        }
    }


  /*  void Awake()
    {
           nearTreeColliders = new Collider[treesRepresentation.Length];

        for (int i = 0; i < treesRepresentation.Length; i++)
        {
            availablesTreesRepresentation.Add(treesRepresentation[i]);
        }
    }

    void Update()
    {
        int cpt = Physics.OverlapSphereNonAlloc(transform.position, range, nearTreeColliders, treeLayerMask);

        for (int i = 0; i < cpt && availablesTreesRepresentation.Count > 0 ; i++)
        {
            GameObject go = availablesTreesRepresentation[0].gameObject;
            AkSoundEngine.PostEvent(forestAmbianceEvent, go, (uint)AkCallbackType.AK_EndOfEvent, UnregisterTree, null);
            availablesTreesRepresentation.RemoveAt(0);
        }

    }

    void UnregisterTree(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if (in_type == AkCallbackType.AK_EndOfEvent)
        {
            AkCallbackManager.AkEventCallbackInfo info = (AkCallbackManager.AkEventCallbackInfo)in_info;


            Debug.Log("tamère");
           // info.
        }

    } */


#if UNITY_EDITOR
 /*       void OnDrawGizmos()
    {
        if (debug)
        {
             Gizmos.DrawWireSphere(transform.position, range);
        }
    } */
#endif
}
