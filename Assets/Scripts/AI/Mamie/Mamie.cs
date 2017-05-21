using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Mamie : AIAgent, IKnifeKillable {
        public Waypoint patrouille;

        bool stun = false;
        private float feedTime;

        // Use this for initialization
        new void Start () {
            base.Start();
            animator = GetComponent<Animator>();

            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if (MoveTo(patrouille.getCurrentDestination(), 3))
                {
                    patrouille.Next();
                    Wait(2);
                }
            };
            root.OnEnd = () =>
            {
                AkSoundEngine.PostEvent("granny_death", gameObject);
                animator.Play("Die");
                state = AIState.DEAD;
            };
            Play(root);
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
            
	    }

        protected void Feeding()
        {
            Vector3 position = patrouille[0];
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if(MoveTo(position, 2))
                {
                    ActionTask feed = new ActionTask();
                    feed.OnExecute = () => 
                    {

                    };
                    feed.OnEnd = () =>
                    {
                        Fabrique();
                    };
                    feed.timer = feedTime;
                    Stop();
                    Play(feed);
                }
            };
            Play(action);
        }
        
        protected void Fabrique()
        {
            Vector3 position = patrouille[1];
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(position, 2))
                {
                    ActionTask make = new ActionTask();
                    make.OnExecute = () =>
                    {

                    };
                    make.OnEnd = () =>
                    {
                        Deposer();
                    };
                    make.timer = feedTime;
                    Stop();
                    Play(make);
                }
            };
            Play(action);
        }

        protected void Deposer()
        {
            Vector3 position = patrouille[2];
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(position, 2))
                {
                    ActionTask make = new ActionTask();
                    make.OnExecute = () =>
                    {

                    };
                    make.OnEnd = () =>
                    {
                        Deposer();
                    };
                    make.timer = feedTime;
                    Stop();
                    Play(make);
                }
            };
            Play(action);
        }

        protected void Echanger()
        {
            Vector3 position = patrouille[1];
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(position, 2))
                {
                    ActionTask make = new ActionTask();
                    make.OnExecute = () =>
                    {

                    };
                    make.OnEnd = () =>
                    {
                        Feeding();
                    };
                    make.timer = feedTime;
                    Stop();
                    Play(make);
                }
            };
            Play(action);
        }

        public override void Kill()
        {
            StopAll();
        }

        public bool IsKnifeVulnerable()
        {
            return stun;
        }

        public void KnifeKill()
        {
            StopAll();
        }

        public void ShowKnifeIcon()
        {
            //throw new NotImplementedException();
        }
    }
}
