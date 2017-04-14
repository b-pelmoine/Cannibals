using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : ActionTask {
    public BBParameter<LayerMask> mask;
    public BBParameter<float> distance;
    public BBParameter<GameObject> target;

    protected override void OnExecute()
    {
        Vector3 position = agent.transform.position;
        Vector3 direction = agent.transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(position,direction, out hit, distance.value, mask.value))
        {
            Debug.Log(hit.collider.gameObject);
            hit.collider.gameObject.GetComponentInParent<GraphOwner>().SendEvent("Death");
        }
        EndAction();
    }
}
