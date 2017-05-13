using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIAgent : MonoBehaviour, IKnifeKillable {
        public enum AIState
        {
            NORMAL,
            VULNERABLE,
            DEAD
        }
        private AIState state = AIState.NORMAL;

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

        protected void Start()
        {
            AIAgentManager.registerAIAgent(this.gameObject);
            if(agent==null)
                agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError(this + ":No navmesh agent in the object.");
            }
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

        /// <summary>
        /// Kill the agent
        /// </summary>
        public virtual void Kill()
        {
            throw new NotImplementedException();
        }

        public bool IsKnifeVulnerable()
        {
            throw new NotImplementedException();
        }

        public void KnifeKill()
        {
            throw new NotImplementedException();
        }

        public void ShowKnifeIcon()
        {
            throw new NotImplementedException();
        }
    }
}
