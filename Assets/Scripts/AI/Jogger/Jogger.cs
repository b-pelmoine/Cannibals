using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Jogger : AIAgent {
        public Waypoint waypoints;
	    
	    new void Start () {
            base.Start();
            if (waypoints == null || waypoints.points.Count==0)
            {
                Debug.LogError("Jogger: Pas de waypoints");
            }
            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if(MoveTo(waypoints.getCurrentDestination(), 3))
                {
                    waypoints.Next();
                }
            };
            Play(root);
	    }

        new void Update()
        {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        }
	
	    
    }
}
