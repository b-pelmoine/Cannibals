using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsFixed : ActionTask<Transform>
{
    public BBParameter<GameObject> target;
    public BBParameter<float> speed;
    public BBParameter<float> angle;
    public BBParameter<float> time;
    public bool repeat;


    protected override void OnExecute() { Rotate(); }
    protected override void OnUpdate() { Rotate(); }

    void Rotate()
    {

        Vector3 agentTarget = target.value.transform.position;
        agentTarget.y = agent.transform.position.y;
        agent.LookAt(agentTarget);
        if(elapsedTime>time.value)
            EndAction();
        /*agentToTarget.y = 0;
        agentToTarget = agentToTarget.normalized;
        if (Vector3.Angle(agentToTarget, agent.forward) > angle.value)
        {
            var dir = agentToTarget;
            agent.rotation = Quaternion.LookRotation(Vector3.RotateTowards(agent.forward, dir, speed.value * Time.deltaTime, 0));
        }
        else {
            if (!repeat)
                EndAction();
        }*/
    }
}

