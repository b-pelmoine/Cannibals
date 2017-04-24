using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAmbiance : MonoBehaviour {

    [SerializeField]
    new Transform transform;

    [SerializeField]
    GameObject[] treesRepresentation;

    [SerializeField]
    float range = 10f;

    [SerializeField]
    LayerMask treeLayerMask;

    [SerializeField]
    string forestAmbianceEvent = "ambiance_forest";


    Collider[] nearTreeColliders;

#if UNITY_EDITOR
    public bool debug = true;
#endif

    void Awake()
    {
        nearTreeColliders = new Collider[treesRepresentation.Length];
    }

    void Update()
    {
        int cpt = Physics.OverlapSphereNonAlloc(transform.position, range, nearTreeColliders, treeLayerMask);

        for (int i = 0; i < cpt; i++)
        {
            AkSoundEngine.PostEvent(forestAmbianceEvent, nearTreeColliders[i].gameObject);
        }

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (debug)
        {
             Gizmos.DrawWireSphere(transform.position, range);
        }
    }
#endif
}
