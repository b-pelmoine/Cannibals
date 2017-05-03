using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Chasseur : AIAgent {
        public float range = 30;
        public bool sawBottle = false;
        public Vector3 bottle;
        public Animator animator;
        NavMeshAgent agent;
        int anim_speedId = Animator.StringToHash("Speed");

        private LineOfSight los;

        public bool alerte = false;
        public bool found_target = false;
        public GameObject shootingTarget;

	    // Use this for initialization
	    void Start () {
            Bottle.OnBottleShaked += OnBottleShaked;
            agent = GetComponent<NavMeshAgent>();
            los = GetComponent<LineOfSight>();
	    }
	
	    // Update is called once per frame
	    void Update () {
		    if(animator != null && agent!=null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            else
            {
                Debug.Log("Chasseur.cs: animator ou navmeshagent non présent");
            }

            if (los.Updated)
            {
                foreach(GameObject obj in los.sighted)
                {
                    Cannibal cannibal = obj.GetComponentInParent<Cannibal>();
                    if (alerte && shootingTarget==null && obj.CompareTag("Player"))
                    {
                        shootingTarget = obj;
                        found_target = true;
                    }
                }
            }
	    }

        void OnBottleShaked(Bottle bot)
        {
            if(Vector3.SqrMagnitude(bot.transform.position-this.transform.position)< range*range && !sawBottle)
            {
                bottle = bot.transform.position;
                sawBottle = true;
            }
        }

        
    }
}
