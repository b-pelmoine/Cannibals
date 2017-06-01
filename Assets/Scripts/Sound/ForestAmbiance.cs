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

    [SerializeField]
    Vector2 capsuleSize= new Vector2(1,2);

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
           CapsuleCollider s = treeCollidersGo.AddComponent<CapsuleCollider>();
            s.height = capsuleSize.y;
            s.radius = capsuleSize.x;
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
        if(!(c is SphereCollider))
        {
            //Debug.LogWarning("! A Tree has no sphere, or an object is on the Tree layer for no reason !");
            return;
        }

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
        if (!(c is CapsuleCollider))
        { 
            return;
        }
 
        for (int i = 0; i < unavailablesGO.Count; i++)
        {
            if (unavailablesGO[i].transform.position == ((CapsuleCollider)c).center)
            {
                AkSoundEngine.StopAll(unavailablesGO[i]);
                availablesGO.Add(unavailablesGO[i]);
                unavailablesGO.RemoveAt(i);
                return;
            }
        }
    } 

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
