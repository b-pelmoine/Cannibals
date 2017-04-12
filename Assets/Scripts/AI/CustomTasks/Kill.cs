using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : ActionTask {
    public BBParameter<GameObject> target;
    public BBParameter<float> distance;
    public BBParameter<float> maxAngle;

    protected override void OnExecute()
    {
        GameObject t = target.value;
        GraphOwner owner = target.value.GetComponentInParent<GraphOwner>();
        Vector3 planarDir = (t.transform.position - agent.transform.position);
        planarDir.y = 0;
        planarDir = planarDir.normalized;
        float angle = Vector3.Angle(agent.transform.forward, planarDir);
        if (owner != null && Vector3.Distance(t.transform.position, agent.transform.position)<distance.value 
            && angle<Mathf.Abs(maxAngle.value))
        {
            owner.SendEvent("Death");
        }
        EndAction();
    }
}
