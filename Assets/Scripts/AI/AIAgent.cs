using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIAgent : MonoBehaviour {
        public enum AIState
        {
            NORMAL,
            VULNERABLE,
            DEAD
        }
        protected AIState state = AIState.NORMAL;

        public AIType type = AIType.UNKNOWN;
        [Range(0,10)]
        public int LevelOfImportance = 0;

        public delegate bool Action();
        Action currentAction = null;
        Action endAction = null;
        float actionElapsed;
        float actionTime;

        public NavMeshAgent agent;
        public Animator animator;
        Vector3? lastRequest;

        protected LineOfSight los;
        private bool detecting = false;

        protected int navMeshMask = 0;

        public LineOfSight LoS
        {
            get
            {
                return los;
            }
        }
        

        protected void Start()
        {
            AIAgentManager.registerAIAgent(this.gameObject);
            if(agent==null)
                agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError(this + ":No navmesh agent in the object.");
            }
            los = GetComponent<LineOfSight>();
        }

        public bool IsIdle()
        {
            return currentAction == null;
        }

        protected void Update()
        {
            actionElapsed += Time.deltaTime;
            if (currentAction != null && (currentAction() || (actionElapsed>=0 && actionElapsed>actionTime)))
            {
                currentAction = null;
                if (endAction != null)
                {
                    endAction();
                    endAction = null;
                }
            }
        }

        public AIState State{
            get
            {
                return state;
            }
        }


        protected void Play(Action act, float timer = -1, Action end = null)
        {
            currentAction = act;
            endAction = end;
            actionElapsed = 0;
            actionTime = timer;
        }

        protected void Stop()
        {
            currentAction = null;
        }

        //Se déplace vers target
        public bool MoveTo(Vector3 target, float stopRadius)
        {
            if(lastRequest==null || lastRequest != target)
            {
                if (!agent.SetDestination(target))
                {
                    lastRequest = null;
                    return false;
                }
            }
            Vector3 distance = (target - transform.position);
            distance.y = 0;
            if (distance.sqrMagnitude < stopRadius * stopRadius)
            {
                agent.ResetPath();
                lastRequest = null;
                return true;
            }
            else
                return false;
        }

        public bool WanderAround(Vector3 target, float wanderRadius, float stopRadius=2)
        {
            if (lastRequest == null)
            {
                Vector3 position = target + new Vector3(UnityEngine.Random.Range(-wanderRadius, wanderRadius), 0, UnityEngine.Random.Range(-wanderRadius, wanderRadius));
                if (agent.SetDestination(position))
                {
                    lastRequest = position;
                }
            }
            Vector3 distance = (lastRequest.Value - transform.position);
            distance.y = 0;
            if (!agent.pathPending && lastRequest.HasValue 
            && (distance.sqrMagnitude <= Mathf.Pow(agent.stoppingDistance,2)  || distance.magnitude < Mathf.Pow(agent.stoppingDistance, 2)))
            {
                agent.ResetPath();
                lastRequest = null;
                return true;
            }
            return false;
        }

        protected void LookAt(Vector3 position)
        {
            position.y = agent.transform.position.y;
            agent.transform.LookAt(position);
        }

        public virtual bool isVulnerable()
        {
            return state == AIState.VULNERABLE;
        }

        /// <summary>
        /// Return true if the agent is dead
        /// </summary>
        public virtual bool isDead()
        {
            return state == AIState.DEAD;
        }

        public void Kill()
        {

        }

        public int GetLevel()
        {
            return LevelOfImportance;
        }
    }
}
