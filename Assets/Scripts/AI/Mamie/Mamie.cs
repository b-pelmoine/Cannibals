using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Mamie : AIAgent {
        public Waypoint patrouille;
        Animator animator;
	    // Use this for initialization
	    new void Start () {
            base.Start();
            animator = GetComponent<Animator>();
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
            if(MoveTo(patrouille.getCurrentDestination(), 3))
            {
                patrouille.Next();
            }
	    }
    }
}
