using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Mamie : AIAgent, IKnifeKillable {

        public enum StartState
        {
            Feeding,
            Generateur
        }

        public StartState startState = StartState.Feeding;

        public GameObject koFX;

        public IconDisplayer icon;
        public ParticleSystem dieParticle;
        public float dieTime = 2;
        public GameObject corpse;

        public Transform mamieHand;

        public float baseSpeed = 2;
        public float runSpeed = 13;

        bool stun = false;
        bool drink = true;


        public Transform positionCanard;
        public float feedTime = 2;
        public float canardSeeRadius = 7;
        public int feedNumber = 4;
        int feeded = 0;

        public Transform positionDeadCanard;

        public float champignonRadius = 15;

        public float machineRadius = 5;
        public float generateurRadius = 5;

        private int machineCanetteNumber = 3;
        private int canetteCounter = 0;

        public Waypoint toMachine;
        public Machine machine;
        public Generateur generateur;
        public Transform positionReserve;

        public Transform positionScout;

        private List<GameObject> carry = new List<GameObject>();

        // Use this for initialization
        new void Start () {
            base.Start();

            Bottle.OnBottleShaked += OnDrink;

            machineCanetteNumber = machine.production;
            machine.finish += Finish;
            ActionTask root = new ActionTask();
            Play(root);
            switch (startState)
            {
                case StartState.Generateur:
                    Fabrique();
                    break;

                case StartState.Feeding:
                default:
                    Feeding();
                    break;
            }
            
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
                if (x.target == null) return false;
                Cannibal c = x.target.GetComponentInParent<Cannibal>();
                return c != null && !c.IsDead();
            });
            if (cannibal != null)
            {
                if (los.getDetectRate(cannibal) == 1)
                    currentTarget = cannibal.target;
                else
                    currentTarget = null;   
                CurrentAction.callData = cannibal;
                return true;
            }
            return false;
        }

        bool CannibalAround()
        {
            SightInfo cannibal = los.FindNearest(x =>
            {
                if (x.target == null) return false;
                Cannibal c = x.target.GetComponentInParent<Cannibal>();
                if (c == null || c.IsDead()) return false;
                Vector3 v = x.target.transform.position;
                v.y = transform.position.y;
                return Vector3.Distance(v, transform.position) < los.radius;
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
                if (champ!=null && champ.linkedCannibal==null && champ.transform.parent!=mamieHand && champ.type==Champignon.Type.Champibon && (best==null || (c.transform.position - transform.position).sqrMagnitude < (best.transform.position - transform.position).sqrMagnitude))
                    best = c.gameObject;
            }
            if (best != null)
            {
                CurrentAction.callData = new SightInfo(best);
                return true;
            }
            return false;
        }

        bool SeeCanard()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, canardSeeRadius);
            Canard best = null;
            foreach(Collider c in cols)
            {
                Canard can = c.GetComponent<Canard>();
                if (can != null && !can.DeadDuck() && (best==null || (can.transform.position - transform.position).sqrMagnitude < (best.transform.position - transform.position).sqrMagnitude))
                 {
                    best = can;
                }
            }
            if (best != null)
            {
                CurrentAction.callData = best;
                return true;
            }
            return false;
        }

        bool SeeDeadCanard()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, canardSeeRadius);
            Canard best = null;
            foreach (Collider c in cols)
            {
                Canard can = c.GetComponent<Canard>();
                if (can != null && can.transform.parent!= mamieHand && can.DeadDuck() && (best == null || (can.transform.position - transform.position).sqrMagnitude < (best.transform.position - transform.position).sqrMagnitude))
                {
                    best = can;
                }
            }
            if (best != null)
            {
                CurrentAction.callData = best;
                return true;
            }
            return false;
        }

        bool SeeScout()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, canardSeeRadius);
            Scout best = null;
            foreach (Collider c in cols)
            {
                Scout can = c.GetComponent<Scout>();
                if (can != null && !can.isDead() && (best == null || (can.transform.position - transform.position).sqrMagnitude < (best.transform.position - transform.position).sqrMagnitude))
                {
                    best = can;
                }
            }
            if (best != null)
            {
                CurrentAction.callData = best;
                return true;
            }
            return false;
        }

        //Actions
        protected void Hit()
        {
            if(!stun)
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
        }

        protected void RamasseChampi()
        {
            GameObject best = (CurrentAction.callData as SightInfo).target;
            if(MoveTo(best.transform.position, 0.5f))
            {
                anim_call_count = 0;
                RotateTowards(best.transform.position, 10, 5).Next = () =>
                {
                    animator.Play("PickUp");
                    AkSoundEngine.SetSwitch("Objects", "Mushrooms", gameObject);
                    AkSoundEngine.PostEvent("granny_objects", gameObject);
                    Wait(0.1f).Next = () =>
                    {
                        Wait(animator.GetCurrentAnimatorStateInfo(0).length).callbacks.Add(0, () => {
                            Grab(best);
                            });
                    };
                };
            }
        }

        protected void FeedCanard()
        {
            Canard target = (CurrentAction.callData as Canard);
            RotateTowards(target.transform.position, 10, 5).Next = () =>
            {
                animator.Play("Give");
                Wait(0.1f).Next = () =>
                {
                    Wait(animator.GetCurrentAnimatorStateInfo(0).length).Next = () =>
                    {
                        target.CallEat();
                        feeded++;
                        if (feeded >= feedNumber)
                            Stop();
                    };
                };
            };
        }

        protected void RamasseCanard()
        {
            Canard target = (CurrentAction.callData as Canard);
            ActionTask task = new ActionTask();
            task.OnExecute = () =>
            {
                if(MoveTo(target.transform.position, 1))
                {
                    anim_call_count = 0;
                    AkSoundEngine.SetSwitch("Objects", "Duck", gameObject);
                    animator.Play("PickUp");
                    Wait(0.1f).Next = () =>
                    {
                        ActionTask pickup = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                        pickup.callbacks.Add(0, () =>
                        {
                            AkSoundEngine.PostEvent("granny_objects", gameObject);
                            Grab(target.gameObject);
                            target.agent.enabled = false;
                            target.enabled = false;
                        });
                        pickup.Next = Stop;
                    };
                }
            };
            Play(task);
        }

        protected void EchangeAvecScout()
        {
            agent.ResetPath();
            Scout scout = (CurrentAction.callData as Scout);
            scout.Call(this);
            Wait(0).callbacks.Add(0, () =>
            {
                Stop();
                anim_call_count = 0;
                LookAt(scout.transform.position);
                animator.Play("Give");
                Wait(0.1f).Next = () =>
                {
                    anim_call_count = 0;
                    ActionTask wait = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                    wait.Next = () =>
                    {
                        scout.Call(10);
                        animator.Play("Give");
                        Wait(0).callbacks.Add(10, () =>
                        {
                            Stop();
                            Wait(1).Next = Feeding;
                        });
                    };
                    wait.callbacks.Add(0, () =>
                    {
                        if (carry.Count > 0)
                            scout.Take(carry[carry.Count - 1]);
                    });
                };
            });
        }

        //Etats
        protected void Feeding()
        {
            Vector3 position = positionCanard.position;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if(MoveTo(position, 2f))
                {
                    feeded = 0;
                    DestroyCarried();
                    ActionTask feed = new ActionTask();
                    feed.Next = GoToMachine;
                    //feed.timer = feedTime;
                    feed.AddReaction(CannibalAround, Hit);
                    feed.AddReaction(SeeCanard, FeedCanard);
                    Stop();
                    Play(feed);
                }
            };
            action.AddReaction(CannibalAround, Hit);
            Play(action);
        }

        protected void RecupereCanard()
        {
            ActionTask task = new ActionTask();
            task.OnExecute = () =>
            {
                if(MoveTo(positionDeadCanard.position, 3))
                {
                    Stop();
                    ActionTask chope = new ActionTask();
                    chope.AddReaction(CannibalAround, Hit);
                    chope.AddReaction(SeeDeadCanard, RamasseCanard);
                    chope.AddReaction(() => !SeeDeadCanard(), Stop);
                    chope.Next = () =>
                    {
                        ActionTask move = new ActionTask();
                        move.OnExecute = () =>
                        {
                            if (MoveTo(machine.transform.position, 1.5f))
                            {
                                anim_call_count = 0;
                                animator.Play("Give");
                                Stop();
                                Wait(0.1f).Next = () =>
                                {
                                    ActionTask t = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                                    t.Next = () =>
                                    {
                                        Fabrique();
                                    };
                                    t.callbacks.Add(0, () =>
                                    {
                                        machine.Fill();
                                        while (carry.Count > 0)
                                            DestroyCarried();
                                    });
                                };
                            }
                        };
                        move.AddReaction(CannibalAround, Hit);
                        Play(move);
                    };
                    Play(chope);
                }
            };
            task.AddReaction(CannibalAround, Hit);
            Play(task);
        }

        protected void GoToMachine()
        {
            ActionTask goingToMachine = new ActionTask();
            toMachine.Reset();
            goingToMachine.OnExecute = () =>
            {
                if (MoveTo(toMachine.getCurrentDestination(), 2))
                {
                    if (toMachine.Next())
                    {
                        Stop();
                    }
                }
            };
            goingToMachine.AddReaction(CannibalAround, Hit);
            goingToMachine.AddReaction(SeeChampi, RamasseChampi);
            goingToMachine.Next = RecupereCanard;
            Play(goingToMachine);
        }
        
        protected void Fabrique()
        {
            Vector3 position = machine.transform.position;

            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(position, 1.5f))
                {
                    RotateTowards(position, 10, 5).Next = () =>
                    {
                        animator.Play("Give");

                        Stop();
                        Wait(0.1f).Next = () =>
                        {
                            anim_call_count = 0;
                            ActionTask wait = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                            wait.callbacks.Add(0, () =>
                            {
                                machine.Launch();
                            });
                            wait.Next = WaitForCanette;
                        };
                    };
                }
            };
            action.AddReaction(
                () => SeeCannibal() && Vector3.Distance((CurrentAction.callData as SightInfo).target.transform.position, position)<machineRadius
                , Hit);
            
            Play(action);
        }

        protected void WaitForCanette()
        {
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(machine.transform.position, 1.5f))
                {
                    if (machine.CanReady())
                    {
                        RamasseCanette();
                    }
                    else
                        RotateTowards(machine.positionCanette.position, 10, 5);
                }
            };
            action.AddReaction(CannibalAround, Hit);
            action.AddReaction(
                () => !machine.IsOn()
                , ActiverGenerateur);
            action.AddReaction(
                () => SeeCannibal() && Vector3.Distance((CurrentAction.callData as SightInfo).target.transform.position, machine.transform.position) < machineRadius
                , Hit);
            //Canette fini
            if (canetteCounter == machineCanetteNumber-1)
                action.Next = () =>
                {
                    EatCanette();
                    canetteCounter = 0;
                };
            else
                action.Next = WaitForCanette;
            Play(action);
        }

        protected void RamasseCanette()
        {
            GameObject can = machine.takeCan();
            ActionTask action = new ActionTask();
            action.OnExecute = () => {
                if(MoveTo(can.transform.position, 1))
                {
                    LookAt(can.transform.position);
                    animator.Play("Give");
                    
                    Wait(0.1f).Next = () =>
                    {
                        anim_call_count = 0;
                        ActionTask take = Wait(animator.GetCurrentAnimatorStateInfo(0).length);
                        take.callbacks.Add(0, () =>
                        {
                            AkSoundEngine.SetSwitch("Objects", "Can", gameObject);
                            AkSoundEngine.PostEvent("granny_objects", gameObject);
                            Grab(can);
                            
                        });
                        take.Next = () => { canetteCounter++; Stop(); };
                    };
                };
                
            };
            action.AddReaction(CannibalAround, Hit);
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
                if (MoveTo(generateur.transform.position, 1f))
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
            action.AddReaction(CannibalAround, Hit);
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
                            Release();
                        });
                        wait.Next = Stop;
                    };
                }
            };
            action.AddReaction(CannibalAround, Hit);
            action.Next = Echanger;
            Play(action);
        }

        protected void EatCanette()
        {
            animator.Play("Drink");
            Wait(0.1f).Next = () =>
            {
                Wait(animator.GetCurrentAnimatorStateInfo(0).length).Next = () =>
                {
                    if (carry.Count > 0)
                    {
                        Canette can = carry[carry.Count - 1].GetComponent<Canette>();
                        if (can != null && can.poisoned)
                        {
                            Die();
                        }
                        else
                        {
                            Deposer();
                        }
                        DestroyCarried();
                    }
                };
            };
        }

        protected void Echanger()
        {
            Vector3 position = positionScout.position;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                MoveTo(position, 0.1f);
            };
            action.AddReaction(CannibalAround, Hit);
            action.AddReaction(SeeScout, EchangeAvecScout);
            Play(action);
        }

        protected void Die()
        {
            Kill();
            AkSoundEngine.PostEvent("granny_death", gameObject);
            animator.Play("Die");
            state = AIState.DEAD;
            Wait(0.1f).Next = () =>
            {
                Wait(animator.GetCurrentAnimatorStateInfo(0).length).Next = () => {
                    Wait(dieTime).Next = () =>
                    {
                        dieParticle.transform.position = transform.position;
                        dieParticle.Play();
                        Instantiate(corpse).transform.position = transform.position;
                        Destroy();
                    };
                };
            };
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
            stun = false;
            Die();
        }

        public void ShowKnifeIcon()
        {
            icon.Show();
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

        public void Take(GameObject g)
        {
            g.transform.parent = mamieHand;
            g.transform.localPosition = Vector3.zero;
            Cookies rigid = g.GetComponent<Cookies>();
            if (rigid != null)
                rigid.m_rigidbody.isKinematic = true;
            carry.Add(g);
            g.SetActive(true);
            Call(10);
        }

        public void Grab(GameObject g)
        {
            g.transform.parent = mamieHand;
            g.transform.localPosition = Vector3.zero;
            if(carry.Count>0)
                carry[carry.Count - 1].SetActive(false);
            Collider c = g.GetComponentInChildren<Collider>();
            if (c != null)
                c.enabled = false;
            carry.Add(g);
        }

        public void Release()
        {
            if (carry.Count > 0)
            {
                GameObject c = carry[carry.Count - 1];
                c.transform.parent = null;
                Rigidbody r = c.GetComponent<Rigidbody>();
                if (r != null) r.isKinematic = false;
                carry.Remove(c);
                if (carry.Count > 0)
                    carry[carry.Count - 1].SetActive(true);
            }
        }

        public void DestroyCarried()
        {
            if(carry.Count > 0)
            {
                GameObject c = carry[carry.Count - 1];
                carry.Remove(c);
                AIAgent agent = c.GetComponent<AIAgent>();
                if (agent != null) agent.Destroy();
                else Destroy(c);
                if (carry.Count > 0)
                    carry[carry.Count - 1].SetActive(true);
            }
        }

        void OnDrink(Bottle bot)
        {
            if (drink && Vector3.Distance(bot.transform.position, transform.position) < los.getSeeDistance())
            {
                ActionTask task = new ActionTask();
                task.OnExecute = () =>
                {
                    if(MoveTo(bot.transform.position, 1.5f))
                    {
                        Stop();
                        agent.ResetPath();
                        bot.linkedCannibal.LooseCannibalObject();
                        Grab(bot.gameObject);
                        ActionTask drinkAnim = PlayAnim("Drink", () => {
                            stun = true;
                            drink = false;
                            koFX.SetActive(true);
                            PlayAnimFor("IdleToKo", 10, () =>
                            {
                                PlayAnim("KoToIdle", () =>
                                {
                                    stun = false;
                                    drink = true;
                                    koFX.SetActive(false);
                                });
                            });
                        });
                    }
                };
                Play(task);
            }
        }
        
    }
}
