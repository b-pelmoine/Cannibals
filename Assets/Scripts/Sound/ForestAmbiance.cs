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

    [SerializeField]
    int ghostTreeCount = 20;

    [SerializeField]
    float range = 20;

    [SerializeField]
    SphereCollider sphereCollider;

    [SerializeField]
    int treeLayer = 31;

    public Terrain terrain;

    /*   List<ForestAmbianceSound> availablesTreesRepresentation = new List<ForestAmbianceSound>();

       Collider[] nearTreeColliders; */

    List<GameObject> availablesGO = new List<GameObject>();

    List<GameObject> unavailablesGO = new List<GameObject>();

    List<Vector3> takenPositions;



#if UNITY_EDITOR
    public bool debug = true;
#endif

    void Start()
    {
        sphereCollider.radius = range;
        TerrainData td = terrain.terrainData;
        GameObject treeCollidersGo = new GameObject("TreeColliders");
        treeCollidersGo.layer = treeLayer;

        for (int i = 0; i < td.treeInstanceCount; i++)
        {
           SphereCollider s = treeCollidersGo.AddComponent<SphereCollider>();
           s.center =  Vector3.Scale(td.treeInstances[i].position, td.size) + terrain.transform.position;
        }

        for(int i=0; i < ghostTreeCount; i++)
        {
            GameObject go = new GameObject("TreeGhost");
            go.transform.SetParent(treeCollidersGo.transform);
            availablesGO.Add(go);
        }
    }

    void Update()
    {
        this.transform.position = Cannibal.BarycentricCannibalPosition();
    }

   /* void Start()
    {
        for(int i=0; i < ghostTreeCount; i++)
        {
            GameObject go = new GameObject("AkPostForestAmbiance");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            availablesGO.Add(go);
        }
    }*/


    /*void Update()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 cannibalBarycentriquePosition = Cannibal.BarycentricCannibalPosition();

        for (int i = unavailablesGO.Count - 1; i >= 0; i--)
        {
            if ((unavailablesGO[i].transform.position - cannibalBarycentriquePosition).sqrMagnitude > range * range)
            {
                availablesGO.Add(unavailablesGO[i]);
                takenPositions.Remove(unavailablesGO[i].transform.position);
                AkSoundEngine.StopAll(unavailablesGO[i]);
                unavailablesGO.RemoveAt(i);
            }
        }

        for (int i = 0; i < terrainData.treeInstanceCount && availablesGO.Count > 0; i++)
        {
            if ((terrainData.treeInstances[i].position - cannibalBarycentriquePosition).sqrMagnitude <= range* range)
            {
                if(!takenPositions.Exists((x)=> { return x == terrainData.treeInstances[i].position; }))
                {
                    takenPositions.Add(terrainData.treeInstances[i].position);
                    AkSoundEngine.PostEvent(forestAmbianceEvent, availablesGO[0]);
                    unavailablesGO.Add(availablesGO[0]);
                    availablesGO.RemoveAt(0);
                }
            }
        }
    }*/


    void OnTriggerEnter(Collider c)
    {
        if (availablesGO.Count > 0)
        {
            availablesGO[0].transform.position = ((SphereCollider)c).center;
            AkSoundEngine.PostEvent(forestAmbianceEvent, availablesGO[0]);
            unavailablesGO.Add(availablesGO[0]);
            availablesGO.RemoveAt(0);
        }
    }

    void OnTriggerExit(Collider c)
    {

        for (int i = 0; i < unavailablesGO.Count; i++)
        {
            if (unavailablesGO[i].transform.position == ((SphereCollider)c).center)
            {
                AkSoundEngine.StopAll(unavailablesGO[i]);
                availablesGO.Add(unavailablesGO[i]);
                unavailablesGO.RemoveAt(i);
                return;
            }
        }
    } 

   /* void Awake()
    {
        TerrainData td = terrain.terrainData;

        for (int i = 0; i < td.treeInstanceCount; i++)
        {
            availablesTreesRepresentation.Add(treesRepresentation[i]);
        }
    }*/

   /* void Update()
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
        void OnDrawGizmos()
    {
        if (debug)
        {
             Gizmos.DrawWireSphere(Cannibal.BarycentricCannibalPosition(), range);
        }
    } 
#endif
}
