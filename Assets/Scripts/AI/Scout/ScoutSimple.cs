using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class ScoutSimple : AIAgent, IKnifeKillable {
        public IconDisplayer icon;
        public List<GameObject> chapeaux;
        Vector3 basePosition;
        public float wanderRadius = 5;
	    // Use this for initialization
	    new void Start () {
            base.Start();
            if (chapeaux.Count > 0)
            {
                int select = UnityEngine.Random.Range(0, chapeaux.Count);
                for(int i = 0; i < chapeaux.Count; i++)
                {
                    if (i == select)
                        chapeaux[i].SetActive(true);
                    else
                        chapeaux[i].SetActive(false);
                }
            }
            basePosition = transform.position;
            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if (WanderAround(basePosition, wanderRadius, 0.2f))
                {
                    Wait(2);
                }
            };
            Play(root);
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
	    }

        public bool IsKnifeVulnerable()
        {
            return true;
        }

        public void KnifeKill()
        {
            animator.Play("Death");
            ResetPath();
            StopAll();
        }

        public void ShowKnifeIcon()
        {
            if (icon != null)
                icon.Show();
        }
    }
}
