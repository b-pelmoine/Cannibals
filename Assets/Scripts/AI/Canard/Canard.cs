using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Canard : AIAgent {
        public Transform wanderPosition;
        public float wanderRadius = 3;
        public Waypoint goTo;

        private bool dead = false;

        public float eatTime = 4;
        public float dieTime = 4;

	    // Use this for initialization
	    new void Start () {
            base.Start();

            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if(WanderAround(wanderPosition.position, wanderRadius))
                {
                    Wait(2);
                }
            };
            root.Next = () =>
            {
                animator.Play("Die");
                Wait(0.1f).Next = () =>
                {
                    Wait(animator.GetCurrentAnimatorStateInfo(0).length).Next = Move;
                };
            };
            Play(root);
	    }

        protected void Eating()
        {
            dead = true;
            agent.ResetPath();
            animator.Play("ToEat");
            Wait(eatTime).Next = () =>
            {
                animator.Play("ToSwim");
                Wait(dieTime).Next = () =>
                {
                    StopAll();
                };
            };
        }

        protected void Move()
        {
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(goTo.getCurrentDestination(), 2))
                {
                    if (goTo.Next())
                    {
                        Stop();
                    }
                }
            };
            Play(action);
        }

        public bool DeadDuck()
        {
            return dead;
        }

        public void CallEat()
        {
            Eating();
        }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
	    }
    }
}
