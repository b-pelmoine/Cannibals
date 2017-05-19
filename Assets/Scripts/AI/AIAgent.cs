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

        protected class Task
        {
            public int id;
            public float elapsed = 0;
            public GameObject target;
            public int count = 0;

            public Task(int _id, GameObject _target = null)
            {
                id = _id;
                target = _target;
            }
        }
        
        protected Stack<Task> tasks = new Stack<Task>();
        public NavMeshAgent agent;
        Vector3? lastRequest;

        protected LineOfSight los;
        private bool detecting = false;

        protected int navMeshMask = 0;

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

        protected void Update()
        {
            if(tasks.Count>0)
                tasks.Peek().elapsed += Time.deltaTime;
        }

        public AIState State{
            get
            {
                return state;
            }
        }

        protected Task CurrentTask
        {
            get
            {
                return tasks.Peek();
            }
        }

        //Se déplace vers target
        protected bool MoveTo(Vector3 target, float stopRadius)
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

        protected bool WanderAround(Vector3 target, float wanderRadius, float stopRadius=2)
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

        protected bool Look()
        {
            Vector3 targetPosition = agent.transform.position;
            targetPosition.y = agent.transform.position.y;
            agent.transform.LookAt(targetPosition);
            //Si le joueur est en vue
            SightInfo si = los.sighted.Find(x => x.target == CurrentTask.target);
            if (si!=null)
            {
                //Si le joueur est détecté -> poursuite
                if (los.getDetectRate(si)>=1.0f)
                {
                    return true;
                }
            }
            else
            {
                CurrentTask.elapsed -= Time.deltaTime * 2;
                if (CurrentTask.elapsed < 0)
                {
                    tasks.Pop();
                }
            }
            return false;
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
    }
}
