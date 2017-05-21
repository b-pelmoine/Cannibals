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
        private float champignonRadius;

        // Use this for initialization
        new void Start () {
            base.Start();
            animator = GetComponent<Animator>();

            ActionTask root = new ActionTask();
            root.OnEnd = () =>
            {
                AkSoundEngine.PostEvent("granny_death", gameObject);
                animator.Play("Die");
                state = AIState.DEAD;
            };
            Play(root);
            Feeding();
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
            
	    }

        //Vue et sens
        bool SeeCannibal()
        {
            SightInfo cannibal = los.FindNearest(x =>
            {
                Cannibal c = x.target.GetComponentInParent<Cannibal>();
                return c != null && !c.IsDead();
            });
            if (cannibal != null)
            {
                CurrentAction.callData = cannibal;
                return true;
            }
            return false;
        }

        bool SeeChampi()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, champignonRadius);
            GameObject best = null;
            foreach (Collider c in cols)
                if ((c.transform.position - transform.position).sqrMagnitude < (best.transform.position - transform.position).sqrMagnitude)
                    best = c.gameObject;
            if (best != null)
            {
                CurrentAction.callData = new SightInfo(best);
                return true;
            }
            return false;
        }

        //Actions
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
                    make.OnEnd = () =>
                    {
                        Deposer();
                    };
                    make.timer = feedTime;
                    Stop();
                    Play(make);
                }
            };
            action.AddReaction(SeeChampi, RamasseChampi);
            Play(action);
        }

        protected void RamasseChampi()
        {
            GameObject best = (CurrentAction.callData as SightInfo).target;
            if(MoveTo(best.transform.position, 2))
            {
                animator.Play("PickUp");
                AkSoundEngine.PostEvent("granny_objects", gameObject);
                Wait(1);
            }
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
