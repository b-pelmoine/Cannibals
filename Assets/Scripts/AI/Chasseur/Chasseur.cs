using System;
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
            
	    }
	
	    // Update is called once per frame
	    new void Update () {
            if (state == AIState.DEAD)
                return;
            base.Update();
            if (bottleTarget != null) return;
            
		    if(animator != null && agent!=null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            else
            {
                Debug.Log("Chasseur.cs: animator ou navmeshagent non présent");
            }

            if (IsIdle())
            {
                currentTarget = null;
                if (Chase())
                {
                    return;
                }
                if(MoveTo(patrouille.getCurrentDestination(), waypointDistance))
                {
                    patrouille.Next();
                    Play(() => false, 4);
                }
            }

            

	    }

        public bool ShootAction()
        {
            LookAt(shootTarget.transform.position);
            return false;
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
                        if (cannibalTarget != cannibal)
                           shootTarget = hit.collider.gameObject;
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

        public bool EndShoot()
        {
            Stop();
            if (cannibalTarget != null)
            {
                if (cannibalTarget.IsDead())
                {
                    Play(Defend, 10);
                }
            }
            return false;
        }

        protected bool Defend()
        {
            if (Chase()) return false;
            WanderAround(shootTarget.transform.position, 5);
            return false;
        }

        protected bool Chase()
        {
            Cannibal can = null;
            List<SightInfo> sightedCannibals = los.sighted.FindAll(x => {
                can = x.target.GetComponentInParent<Cannibal>();
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
                    currentTarget = bestTarget.target;
                    agent.ResetPath();
                    animator.Play("Shoot");
                    shootTarget = bestTarget.target;
                    cannibalTarget = bestTarget.target.GetComponentInParent<Cannibal>();
                    Play(ShootAction, 7, EndShoot);
                }
                else
                {
                    if (los.getDetectRate(bestTarget) >= 1)
                    {
                        currentTarget = bestTarget.target;
                        agent.speed = runSpeed;
                        MoveTo(bestTarget.target.transform.position, 1);
                    }
                    else
                    {
                        currentTarget = null;
                        agent.speed = walkSpeed;
                        MoveTo(bestTarget.target.transform.position, 1);
                    }
                }
                return true;
            }
            return false;
        }

        protected bool GoTo()
        {
            if (Chase())
                return true;
            if (MoveTo(shootTarget.transform.position, 3)) return true;
            return false;
        }

        protected bool Drink()
        {
            if (MoveTo(bottleTarget.transform.position, 1))
            {
                bottleTarget = null;
                Stop();
                animator.Play("Drink");
                AkSoundEngine.PostEvent("hunter_drink", gameObject);
                stun = true;
                Play(() => false, stunTime, Resurrect);
            }
            return false;
        }

        protected bool Resurrect()
        {
            stun = false;
            animator.Play("Resurrect");
            Stop();
            return true;
        }

        /// <summary>
        /// The dog calls the hunter
        /// </summary>
        public void Call(GameObject target)
        {
            shootTarget = target;
            Play(GoTo);
        }

        void OnBottleShaked(Bottle bot)
        {
            if (Vector3.SqrMagnitude(bot.transform.position - this.transform.position) < Mathf.Pow(los.getSeeDistance(),2))
            {
                bottleTarget = bot;
                if(IsIdle())
                    Play(Drink);
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
            AkSoundEngine.PostEvent("hunter_death", gameObject);
            animator.Play("Death");
            state = AIState.DEAD;
        }

        public void ShowKnifeIcon()
        {
            //throw new NotImplementedException();
        }
    }
}
