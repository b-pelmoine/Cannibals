using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIChien : AIAgent {

        public Chasseur hunter;


        int anim_speedId = Animator.StringToHash("Speed");

        public float eatingTime = 3;
        public float stopTime = 3;
        public float wanderDistance = 3;
        public float barkDistance = 7;

        GameObject target = null;
        Cannibal cannibalTarget = null;



        // Use this for initialization
        new void Start () {
            base.Start();
            type = AIType.Dog;
            //AkSoundEngine.PostEvent("dog_idle", gameObject);
        }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            if (animator != null && agent != null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            if (!IsIdle()) return;
            if (AnalyseSight())
                return;
            if(WanderAround(hunter.transform.position, wanderDistance))
            {
                Play(() => false, 2);
            }
	    }

        bool EatEnd()
        {
            animator.Play("EatToIdle");
            target.gameObject.SetActive(false);
            target = null;
            return false;
        }

        bool Bark()
        {
            if (MoveTo(target.transform.position, barkDistance))
            {
                LookAt(target.transform.position);
                animator.Play("Bark");
                hunter.Call(target);
                Stop();
                Play(() => false, 1);
            }
            return false;
        }

        bool AnalyseSight()
        {
            foreach (SightInfo obj in los.sighted)
            {
                if (obj.target.GetComponent<Bone>() != null)
                {
                    if (MoveTo(obj.target.transform.position, 2))
                    {
                        animator.Play("IdleToEat");
                        target = obj.target;
                        Play(() => false, 7, EatEnd);
                    }
                    return true;
                }
                else if (obj.target.CompareTag("Player"))
                {
                    Cannibal can = obj.target.GetComponentInParent<Cannibal>();
                    if (!can.IsDead())
                    {
                        target = obj.target;
                        Play(Bark);
                        return true;
                    }
                }
                else
                {
                    Bush buisson = obj.target.GetComponent<Bush>();
                    if (buisson!=null && buisson.IsMoving())
                    {
                        target = obj.target;
                        Play(Bark);
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
