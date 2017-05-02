using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        void Start()
        {
            AIAgentManager.registerAIAgent(this);
        }

        public AIState State{
            get
            {
                return state;
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
