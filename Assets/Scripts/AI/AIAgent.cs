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

        protected int anim_call_count = 0;

        public class ActionTask
        {
            public delegate void Action();
            public delegate bool Condition();
            public class Reaction
            {
                public Condition pred = null;
                public Action action = null;
                public Reaction(Condition c, Action a)
                {
                    pred = c;
                    action = a;
                }
            }
            public Action OnExecute = null;
            public Action OnBegin = null;
            public Action OnEnd = null;
            public Action Next = null;
            public List<Reaction> reaction = new List<Reaction>();
            public Dictionary<int, Action> callbacks = new Dictionary<int, Action>();
            public object callData = null;
            public float elapsed = 0;
            public float timer = 0;
            public GameObject target = null;

            public void AddReaction(Condition c, Action a)
            {
                reaction.Add(new Reaction(c, a));
            }
        }

        Stack<ActionTask> actions = new Stack<ActionTask>();

        public NavMeshAgent agent;
        public Animator animator;
        Vector3? lastRequest;

        protected LineOfSight los;

        protected int navMeshMask = 0;

        protected GameObject currentTarget = null;

        public LineOfSight LoS
        {
            get
            {
                return los;
            }
        }

        public GameObject getCurrentTarget()
        {
            return currentTarget;
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

        protected void Update()
        {
            
            if (actions.Count==0) return;
            CurrentAction.elapsed += Time.deltaTime;
            if (CurrentAction.timer > 0 && CurrentAction.elapsed > CurrentAction.timer)
                Stop();
            if (CurrentAction == null) return;
            foreach (ActionTask.Reaction r in CurrentAction.reaction)
            {
                if (r.pred())
                {
                    r.action();
                    return;
                }
            }
            if(CurrentAction.OnExecute!=null)
                CurrentAction.OnExecute();
            
        }

        public AIState State{
            get
            {
                return state;
            }
        }

        protected ActionTask CurrentAction
        {
            get
            {
                if (actions.Count == 0) return null;
                return actions.Peek();
            }
        }

        protected void Play(ActionTask act)
        {
            actions.Push(act);
            if (act.OnBegin != null)
                act.OnBegin();
        }

        protected void Stop()
        {
            ActionTask.Action next = CurrentAction.Next;
            if(CurrentAction.OnEnd!=null)
                CurrentAction.OnEnd();
            actions.Pop();
            if (next != null)
                next();
        }

        protected void StopAll()
        {
            while (CurrentAction != null) Stop();
        }

        public void Call(int id)
        {
            if (CurrentAction!=null && CurrentAction.callbacks.ContainsKey(id))
                CurrentAction.callbacks[id]();
        }

        public void AnimCall()
        {
            Call(anim_call_count);
            anim_call_count++;
        }

        protected ActionTask Wait(float time, ActionTask.Action execute=null, ActionTask.Action end = null)
        {
            ActionTask wait = new ActionTask();
            wait.timer = time;
            wait.OnExecute = execute;
            wait.OnEnd = end;
            Play(wait);
            return wait;
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

        public virtual void Kill()
        {

        }

        public int GetLevel()
        {
            return LevelOfImportance;
        }
    }
}
