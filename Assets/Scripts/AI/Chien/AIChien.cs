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
            ActionTask brain = new ActionTask();
            brain.OnExecute = () =>
            {
                if (AnalyseSight())
                    return;
                if (WanderAround(hunter.transform.position, wanderDistance))
                {
                    ActionTask task = new ActionTask();
                    task.timer = 2;
                    task.OnExecute = () => AnalyseSight();
                    task.callbacks.Add(0, EatBone);
                    task.callbacks.Add(1, Bark);
                    Play(task);
                }
            };
            brain.callbacks.Add(0, EatBone);
            brain.callbacks.Add(1, Bark);
            Play(brain);
        }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            if (animator != null && agent != null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }  
	    }



        void Bark()
        {
            GameObject target = CurrentAction.callData as GameObject;

            if (MoveTo(target.transform.position, barkDistance))
            {
                LookAt(target.transform.position);
                animator.Play("Bark");
                hunter.Call(target);
                ActionTask wait = new ActionTask();
                wait.timer = 1;
                Play(wait);
            }
        }


        void EatBone()
        {
            Bone target = (CurrentAction.callData as Bone);
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(target.transform.position, 2))
                {
                    Stop();
                    animator.Play("IdleToEat");
                    ActionTask task = new ActionTask();
                    task.timer = 7;
                    task.OnEnd = () =>
                    {
                        animator.Play("EatToIdle");
                        if (target.linkedCannibal != null)
                            target.linkedCannibal.LooseCannibalObject();
                        target.gameObject.SetActive(false);
                    };
                    Play(task);
                }
            };
            Play(action);
        }

        bool AnalyseSight()
        {
            foreach (SightInfo obj in los.sighted)
            {
                Bone bone = obj.target.GetComponent<Bone>();
                if (bone != null)
                {
                    CurrentAction.callData = bone;
                    Call(0);
                    return true;
                }
                else if (obj.target.CompareTag("Player"))
                {
                    Cannibal can = obj.target.GetComponentInParent<Cannibal>();
                    if (!can.IsDead())
                    {
                        CurrentAction.callData = obj.target;
                        Call(1);
                        return true;
                    }
                }
                else
                {
                    Bush buisson = obj.target.GetComponent<Bush>();
                    if (buisson!=null && buisson.IsMoving())
                    {
                        CurrentAction.callData = obj.target;
                        Call(1);
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
