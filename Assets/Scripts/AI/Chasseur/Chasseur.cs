﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Chasseur : AIAgent, IKnifeKillable {
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
            action.callbacks.Add(0, GoTo);
            action.callbacks.Add(1, Drink);
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
            if (!Chase())
            {
                if (MoveTo(patrouille.getCurrentDestination(), waypointDistance))
                {
                    patrouille.Next();
                    //S'arrete 4 sec
                    ActionTask act = new ActionTask();
                    act.timer = 4;
                    act.OnExecute= () => Chase();
                    Play(act);
                }
            }
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

        protected void Defend(GameObject target)
        {
            ActionTask defend = new ActionTask();
            defend.target = target;
            defend.OnExecute = () =>
            {
                if (Chase()) return;
                if (WanderAround(target.transform.position, defendRange, 1))
                {
                    ActionTask wait = new ActionTask();
                    wait.OnExecute = () => Chase();
                    wait.timer = 3;
                    Play(wait);
                }
            };
            defend.callbacks.Add(0, GoTo);
            defend.callbacks.Add(1, Drink);
            defend.timer = defendTime;
            Play(defend);
        }

        protected bool Chase()
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
                if (los.getDetectRate(bestTarget)>=1 && (bestTarget.target.transform.position - transform.position).sqrMagnitude <= chaseShootDistance * chaseShootDistance)
                {
                    //Shoot at target
                    ActionTask shoot = new ActionTask();
                    shoot.target = bestTarget.target;
                    shoot.OnBegin = () =>
                    {
                        agent.ResetPath();
                        animator.Play("Shoot");
                    };
                    shoot.OnExecute = () => LookAt(CurrentAction.target.transform.position);
                    currentTarget = bestTarget.target;
                    Play(shoot);
                    return true;
                }
                if (los.getDetectRate(bestTarget) >= 1)
                {
                    //Run after player
                    currentTarget = bestTarget.target;
                    agent.speed = runSpeed;
                    MoveTo(bestTarget.target.transform.position, 1);
                    return true;
                }
                currentTarget = null;
                agent.speed = walkSpeed;
                MoveTo(bestTarget.target.transform.position, 1);
                return true;
            }
            return false;
        }

        protected void GoTo()
        {
            Vector3 position = (CurrentAction.callData as GameObject).transform.position;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
             {
                 if (Chase()) return;
                 if (MoveTo(position, 3))
                 {
                     Stop();
                     ActionTask wait = new ActionTask();
                     wait.timer=2;
                     wait.OnExecute = () => Chase();
                     wait.callbacks.Add(0, CurrentAction.callbacks[0]);
                     wait.callbacks.Add(1, Drink);
                     Play(wait);
                 }
             };
            action.callbacks.Add(0, CurrentAction.callbacks[0]);
            action.callbacks.Add(1, Drink);
            Play(action);
        }

        protected void Drink()
        {
            Bottle bottle = CurrentAction.callData as Bottle;

            ActionTask drink = new ActionTask();
            drink.OnExecute = () =>
            {
                if (MoveTo(bottle.transform.position, 2))
                {
                    animator.Play("Drink");
                    AkSoundEngine.PostEvent("hunter_drink", gameObject);
                    stun = true;
                    if(bottle.linkedCannibal!=null)
                        bottle.linkedCannibal.LooseCannibalObject();
                    bottle.gameObject.SetActive(false);
                    ActionTask action = new ActionTask();
                    action.timer = stunTime;
                    action.OnEnd = Resurrect;
                    Play(action);
                }
            };
            Play(drink);           
        }

        protected void Resurrect()
        {
            stun = false;
            animator.Play("Resurrect");
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
                Call(0);
            }
        }

        void OnBottleShaked(Bottle bot)
        {
            if (Vector3.SqrMagnitude(bot.transform.position - this.transform.position) < Mathf.Pow(los.getSeeDistance(),2))
            {
                if (CurrentAction != null)
                {
                    CurrentAction.callData = bot;
                    Call(1);
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

        public void ShowKnifeIcon()
        {
            //throw new NotImplementedException();
        }
    }
}
