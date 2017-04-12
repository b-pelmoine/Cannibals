using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsFixed : ActionTask {
    public BBParameter<GameObject> target;
    public BBParameter<float> speed;
    public BBParameter<float> angle;

    protected override void OnExecute()
    {
        Vector3 agentToTarget = (target.value.transform.position - agent.transform.position);
        agentToTarget.y = 0;
        agentToTarget = agentToTarget.normalized;
        float currentAngle = Vector3.Angle(agentToTarget, agent.transform.forward);
        if (currentAngle < angle.value)
            EndAction();
        else
            agent.transform.Rotate(Vector3.up, currentAngle > 0 ? speed.value : -speed.value);

    }
}
