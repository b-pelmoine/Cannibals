using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Chasseur : AIAgent, IKnifeKillable {
        public IconDisplayer icon;
        public Transform hand;
        public float walkSpeed = 3;
        public float runSpeed = 6;
        public float waypointDistance = 2;
        public float chaseShootDistance = 10;
        public float shootingRange = 1;
        public LayerMask shootingMask = 0;
        
        public float lostTargetTime = 3;
        public float defendTime = 5;
        public float defendRange = 10;
        
        public ParticleSystem fireBurst;

        int anim_speedId = Animator.StringToHash("Speed");
        int anim_shoot = Animator.StringToHash("Shoot");

        GameObject shootTarget = null;
        Cannibal cannibalTarget = null;

        public bool alerte = false;

        public Waypoint patrouille;
        public float stunTime = 10;
        private bool stun = false;

        Bottle bottleTarget = null;

        const int BOTTLE_CALL = 0;
        const int DOG_CALL = 1;


        // Use this for initialization
        new void Start () {
            base.Start();
            type = AIType.Hunter;
            Bottle.OnBottleShaked += OnBottleShaked;

            //Initialisation de la racine
            ActionTask action = new ActionTask();
            action.OnExecute = Brain;
            action.OnEnd = () =>
            {
                AkSoundEngine.PostEvent("hunter_death", gameObject);
                animator.Play("Death");
                state = AIState.DEAD;
            };
            action.AddReaction(SeeCannibal, Chase);
            action.callbacks.Add(DOG_CALL, GoTo);
            action.callbacks.Add(BOTTLE_CALL, Drink);
            Play(action);
	    }
	
	    // Update is called once per frame
	    new void Update () {
            if (state == AIState.DEAD)
                return;
            base.Update();
            
		    if(animator != null && agent!=null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            else
            {
                Debug.Log("Chasseur.cs: animator ou navmeshagent non présent");
            }
	    }

        public void Brain()
        {
            if (MoveTo(patrouille.getCurrentDestination(), waypointDistance))
            {
                patrouille.Next();
                //S'arrete 4 sec
                Wait(4).AddReaction(SeeCannibal, Chase);
            }
        }

        protected void Defend(GameObject target)
        {
            ActionTask defend = new ActionTask();
            defend.target = target;
            defend.OnExecute = () =>
            {
                if (WanderAround(target.transform.position, defendRange, 1))
                {
                   Wait(3).AddReaction(SeeCannibal, Chase);
                }
            };
            defend.AddReaction(SeeCannibal, Chase);
            defend.callbacks.Add(DOG_CALL, GoTo);
            defend.callbacks.Add(BOTTLE_CALL, Drink);
            defend.timer = defendTime;
            Play(defend);
        }

        protected bool SeeCannibal()
        {
            List<SightInfo> sightedCannibals = los.sighted.FindAll(x => {
                Cannibal can = x.target.GetComponentInParent<Cannibal>();
                return can != null && !can.IsDead();
            });
            SightInfo bestTarget = null;
            foreach (SightInfo si in sightedCannibals)
            {
                if (bestTarget == null || ((si.target.transform.position - transform.position).sqrMagnitude < (bestTarget.target.transform.position - transform.position).sqrMagnitude))
                {
                    bestTarget = si;
                }
            }
            if (bestTarget != null)
            {
                CurrentAction.callData = bestTarget;
                return true;
            }
            return false;
        }

        protected void Chase()
        {
            SightInfo bestTarget = CurrentAction.callData as SightInfo;
            if (bestTarget != null)
            {
                if (los.getDetectRate(bestTarget)>=1 && (bestTarget.target.transform.position - transform.position).sqrMagnitude <= chaseShootDistance * chaseShootDistance)
                {
                    //Shoot at target
                    ActionTask shoot = new ActionTask();
                    shoot.target = bestTarget.target;
                    agent.ResetPath();
                    animator.Play("Shoot");
                    shoot.OnExecute = () => LookAt(CurrentAction.target.transform.position);
                    Wait(0.1f).Next = () =>
                    {
                        shoot.timer = animator.GetCurrentAnimatorStateInfo(0).length;
                        Play(shoot);
                    };
                    currentTarget = bestTarget.target;
                    return;
                }
                if (los.getDetectRate(bestTarget) >= 1)
                {
                    //Run after player
                    currentTarget = bestTarget.target;
                    agent.speed = runSpeed;
                    MoveTo(bestTarget.target.transform.position, 1);
                    return;
                }
                currentTarget = null;
                agent.speed = walkSpeed;
                MoveTo(bestTarget.target.transform.position, 1);
                return;
            }
            return;
        }

        protected void GoTo()
        {
            Vector3 position = (CurrentAction.callData as GameObject).transform.position;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
             {
                 if (MoveTo(position, 3))
                 {
                     Stop();
                     ActionTask wait = new ActionTask();
                     wait.timer=2;
                     wait.AddReaction(SeeCannibal, Chase);
                     wait.callbacks.Add(DOG_CALL, CurrentAction.callbacks[DOG_CALL]);
                     wait.callbacks.Add(BOTTLE_CALL, Drink);
                     Play(wait);
                 }
             };
            action.AddReaction(SeeCannibal, Chase);
            action.callbacks.Add(DOG_CALL, CurrentAction.callbacks[DOG_CALL]);
            action.callbacks.Add(BOTTLE_CALL, Drink);
            Play(action);
        }

        protected void Drink()
        {
            Bottle bottle = CurrentAction.callData as Bottle;
            AkSoundEngine.PostEvent("hunter_bottle", gameObject);
            ActionTask drink = new ActionTask();
            drink.OnExecute = () =>
            {
                if (MoveTo(bottle.transform.position, 2))
                {
                    Stop();
                    AkSoundEngine.PostEvent("hunter_drink", gameObject);
                    if(bottle.linkedCannibal!=null)
                        bottle.linkedCannibal.LooseCannibalObject();
                    bottle.transform.parent = hand;
                    PlayAnim("Drink", () =>
                    {
                        bottle.gameObject.SetActive(false);
                        stun = true;
                        ActionTask action = new ActionTask();
                        action.timer = stunTime;
                        action.Next = Resurrect;
                        Play(action);
                    });
                }
            };
            Play(drink);           
        }

        public void Shoot()
        {
            AkSoundEngine.PostEvent("hunter_rifle", gameObject);
            fireBurst.Play();
            Vector3 position = agent.transform.position;
            Vector3 direction = agent.transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(position, direction, out hit, shootingRange, shootingMask))
            {
                AIAgent agent = hit.collider.gameObject.GetComponent<AIAgent>();
                if (agent != null)
                {
                    agent.Kill();
                }
                else
                {
                    Cannibal cannibal = hit.collider.gameObject.GetComponentInParent<Cannibal>();
                    if (cannibal != null)
                    {
                        CurrentAction.target = hit.collider.gameObject;
                        cannibal.Kill();
                    }
                    else
                    {
                        Bush bush = hit.collider.gameObject.GetComponent<Bush>();
                        if (bush != null)
                        {
                            bush.KillACannibal();
                        }
                    }
                }
            }
        }

        public void EndShoot()
        {
            GameObject target = CurrentAction.target;
            Stop();
            Cannibal can = target.GetComponentInParent<Cannibal>();
            if (can != null && can.IsDead())
            {
                Defend(target);
            }
        }

        protected void Resurrect()
        {
            stun = false;
            PlayAnim("Resurrect");
        }

        /// <summary>
        /// The dog calls the hunter
        /// </summary>
        public void Call(GameObject target)
        {
            shootTarget = target;
            if (CurrentAction != null)
            {
                CurrentAction.callData = target;
                Call(DOG_CALL);
            }
        }

        void OnBottleShaked(Bottle bot)
        {
            if (Vector3.SqrMagnitude(bot.transform.position - this.transform.position) < Mathf.Pow(los.getSeeDistance(),2))
            {
                if (CurrentAction != null)
                {
                    CurrentAction.callData = bot;
                    Call(BOTTLE_CALL);
                }
            }
        }

        void FootSteps()
        {
            AkSoundEngine.PostEvent("hunter_steps", gameObject);
        }

        public bool IsKnifeVulnerable()
        {
            if (stun)
                return true;
            return false;
        }

        public void KnifeKill()
        {
            StopAll();
        }

        public override void Kill()
        {
            StopAll();
        }

        public void ShowKnifeIcon()
        {
            icon.Show();
        }
    }
}
