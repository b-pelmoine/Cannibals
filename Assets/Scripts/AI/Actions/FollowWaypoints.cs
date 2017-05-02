using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Name("Follow waypoints")]
[Category("Movement")]
public class FollowWaypoints : ActionTask<UnityEngine.AI.NavMeshAgent>
{
    public BBParameter<Waypoint> waypoint;
    public BBParameter<float> speed = 3;
    public float keepDistance = 0.1f;

    private Vector3? lastRequest;
    private Vector3? targetPosition=null;
 
    protected override string info
    {
        get { return "Follow Waypoints"; }
    }

    protected override void OnExecute()
    {
        targetPosition = waypoint.value.getCurrentDestination();
        agent.speed = speed.value;
        if ((agent.transform.position - targetPosition.Value).magnitude < agent.stoppingDistance + keepDistance)
        {
            End(true);
            return;
        }

        Go();
    }

    protected override void OnUpdate()
    {
        Go();
    }

    void Go()
    {

        if (lastRequest != targetPosition)
        {
            if (!agent.SetDestination(targetPosition.Value))
            {
                End(false);
                return;
            }
        }

        lastRequest = targetPosition;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance)
        {
            End(true);
        }
    }

    protected override void OnStop()
    {
        if (lastRequest != null && agent.gameObject.activeSelf)
        {
            agent.ResetPath();
        }
        lastRequest = null;
    }

    protected override void OnPause()
    {
        OnStop();
    }

    private void End(bool result = true)
    {
        EndAction(result);
        waypoint.value.Next();
    }
}