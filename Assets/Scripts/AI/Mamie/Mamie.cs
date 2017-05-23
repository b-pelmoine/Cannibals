using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Mamie : AIAgent, IKnifeKillable {
        public Waypoint patrouille;

        

        public Transform mamieHand;

        public float baseSpeed = 2;
        public float runSpeed = 13;

        bool stun = false;
        public float feedTime = 2;
        public float champignonRadius = 15;

        public float machineRadius = 5;
        public float generateurRadius = 5;

        public int machineCanetteNumber = 3;
        private int canetteCounter = 0;


        public Machine machine;
        public Generateur generateur;
        public Transform positionReserve;

        private List<GameObject> carry = new List<GameObject>();

        // Use this for initialization
        new void Start () {
            base.Start();
            machine.finish += Finish;
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
            agent.speed = baseSpeed;
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
            {
                Champignon champ = c.gameObject.GetComponent<Champignon>();
                if (champ!=null && champ.type==Champignon.Type.Champibon && (best==null || (c.transform.position - transform.position).sqrMagnitude < (best.transform.position - transform.position).sqrMagnitude))
                    best = c.gameObject;
            }
            if (best != null)
            {
                CurrentAction.callData = new SightInfo(best);
                return true;
            }
            return false;
        }

        //Actions
        protected void Hit()
        {
            agent.speed = runSpeed;
            SightInfo target = CurrentAction.callData as SightInfo;
            if(MoveTo(target.target.transform.position, 2))
            {
                animator.Play("Hit");
                AkSoundEngine.PostEvent("granny_whoosh_hit", gameObject);
                Wait(0.1f).Next = () =>
                {
                    anim_call_count = 0;
                    Wait(animator.GetCurrentAnimatorStateInfo(0).length,
                    () => LookAt(target.target.transform.position)).callbacks.Add(0, () =>
                    {
                        
                        if ((target.target.transform.position - transform.position).sqrMagnitude < 2 * 2)
                        {
                            AkSoundEngine.PostEvent("granny_hit", gameObject);
                            Cannibal can = target.target.GetComponentInParent<Cannibal>();
                            if (can != null) can.Kill();
                        }
                    });
                };
            }
        }

        protected void RamasseChampi()
        {
            GameObject best = (CurrentAction.callData as SightInfo).target;
            if(MoveTo(best.transform.position, 2))
            {
                LookAt(best.transform.position);
                animator.Play("PickUp");
                AkSoundEngine.SetSwitch("Objects", "Mushrooms", gameObject);
                AkSoundEngine.PostEvent("granny_objects", gameObject);
                Wait(0.1f).Next = () =>
                {
                    Wait(animator.GetCurrentAnimatorStateInfo(0).length).callbacks.Add(0, () => best.SetActive(false));
                };
            }
        }

        //Etats
        protected void Feeding()
        {
            Vector3 position = patrouille[0];
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if(MoveTo(position, 2))
                {
                    ActionTask feed = new ActionTask();
                    feed.Next = Fabrique;
                    feed.timer = feedTime;
                    Stop();
                    Play(feed);
                }
            };
            action.AddReaction(SeeCannibal, Hit);
            Play(action);
        }
        
        protected void Fabrique()
        {
            Vector3 position = machine.transform.position;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(position, 2))
                {
                    animator.Play("Give");
                    
                    Stop();
                    Wait(0.1f).Next = () =>
                    {
                        anim_call_count = 0;
                        ActionTask wait = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                        wait.callbacks.Add(0, machine.Launch);
                        wait.Next = WaitForCanette;
                    };
                }
            };
            action.AddReaction(
                () => SeeCannibal() && Vector3.Distance((CurrentAction.callData as SightInfo).target.transform.position, position)<machineRadius
                , Hit);
            action.AddReaction(SeeChampi, RamasseChampi);
            Play(action);
        }

        protected void WaitForCanette()
        {
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if(MoveTo(machine.transform.position, 3))
                    LookAt(machine.transform.position);
            };
            action.AddReaction(
                () => !machine.IsOn()
                , ActiverGenerateur);
            action.AddReaction(
                () => SeeCannibal() && Vector3.Distance((CurrentAction.callData as SightInfo).target.transform.position, machine.transform.position) < machineRadius
                , Hit);
            //Canette fini
            action.callbacks.Add(0, () =>
            {
                RamasseCanette();
            });
            if (canetteCounter == machineCanetteNumber-1)
                action.Next = () =>
                {
                    Deposer();
                    canetteCounter = 0;
                };
            else
                action.Next = Fabrique;
            Play(action);
        }

        protected void RamasseCanette()
        {
            GameObject can = CurrentAction.callData as GameObject;
            ActionTask action = new ActionTask();
            action.OnExecute = () => {
                if(MoveTo(can.transform.position, 1))
                {
                    animator.Play("Give");
                    
                    Wait(0.1f).Next = () =>
                    {
                        anim_call_count = 0;
                        ActionTask take = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                        take.callbacks.Add(0, () =>
                        {
                            AkSoundEngine.SetSwitch("Objects", "Can", gameObject);
                            AkSoundEngine.PostEvent("granny_objects", gameObject);
                            can.transform.parent = mamieHand;
                            can.transform.localPosition = Vector3.zero;
                            canetteCounter++;
                            carry.Add(can);
                            if (canetteCounter < machineCanetteNumber)
                                can.SetActive(false);
                        });
                        take.Next = Stop;
                    };
                };
                
            };
            action.AddReaction(
                () => SeeCannibal() && Vector3.Distance((CurrentAction.callData as SightInfo).target.transform.position, machine.transform.position) < machineRadius
                , Hit);
            action.Next = Stop;
            Play(action);
        }

        protected void ActiverGenerateur()
        {
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(generateur.transform.position, 2))
                {
                    animator.Play("Give");
                    Wait(0.1f).Next = () =>
                    {
                        anim_call_count = 0;
                        ActionTask wait = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                        wait.callbacks.Add(0, generateur.Switch);
                        wait.Next = Stop;
                    };
                }
            };
            action.AddReaction(
                () => SeeCannibal() && Vector3.Distance((CurrentAction.callData as SightInfo).target.transform.position, generateur.transform.position) < generateurRadius
                , Hit);
            Play(action);
        }

        protected void Deposer()
        {
            Vector3 position = positionReserve.position;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(position, 0.2f))
                {
                    animator.Play("PickUp");
                    Wait(0.1f).Next = () => {
                        anim_call_count = 0;
                        ActionTask wait = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                        wait.callbacks.Add(0, () =>
                        {
                            GameObject c = carry[carry.Count - 1];
                            c.transform.parent = null;
                            Rigidbody rigid = c.GetComponent<Rigidbody>();
                            if (rigid != null) rigid.isKinematic = false;
                            carry.Remove(c);
                        });
                        wait.Next = Stop;
                    };
                }
            };
            action.AddReaction(SeeCannibal, Hit);
            action.Next = Echanger;
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
                    make.Next = Feeding;
                    make.timer = feedTime;
                    Stop();
                    Play(make);
                }
            };
            action.AddReaction(SeeCannibal, Hit);
            Play(action);
        }

        //Calls
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

        //Callback Machine
        public void Finish(GameObject can)
        {
            CurrentAction.callData = can;
            Call(0);
        }

        public void Call()
        {
            if(CurrentAction.callbacks.ContainsKey(anim_call_count))
                Call(anim_call_count++);
        }
        
    }
}
