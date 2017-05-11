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
        static UnityEngine.Random rand = new UnityEngine.Random();

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
        protected NavMeshAgent agent;
        Vector3? lastRequest;

        protected void Start()
        {
            AIAgentManager.registerAIAgent(this.gameObject);
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
                    return true;
                }
            }
            if ((target - transform.position).sqrMagnitude < stopRadius * stopRadius)
            {
                agent.ResetPath();
                lastRequest = null;
                return true;
            }
            else
                return false;
        }

        protected void WanderAround(Vector3 target, float wanderRadius)
        {
            if (lastRequest == null)
            {
                Vector3 position = target + new Vector3(UnityEngine.Random.Range(-wanderRadius, wanderRadius), 0, UnityEngine.Random.Range(-wanderRadius, wanderRadius));
                if (agent.SetDestination(position))
                {
                    lastRequest = position;
                }
            }
            if(!agent.pathPending && lastRequest.HasValue && agent.stoppingDistance >= (lastRequest.Value - transform.position).magnitude)
            {
                agent.ResetPath();
                lastRequest = null;
            }
        }

        public virtual bool isVulnerable()
        {
            return state == AIState.VULNERABLE;
        }

        public virtual bool isDead()
        {
            return state == AIState.DEAD;
        }

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
