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
            AI.AIAgent agent = hit.collider.gameObject.GetComponent<AI.AIAgent>();
            if (agent != null)
            {
                agent.Kill();
            }
            else
            {
                Cannibal cannibal = hit.collider.gameObject.GetComponentInParent<Cannibal>();
                if (cannibal != null)
                {
                    cannibal.Kill();
                }
            }
        }
        EndAction();
    }
}
