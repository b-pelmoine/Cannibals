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
                if (WanderAround(hunter.transform.position, wanderDistance))
                {
                    ActionTask task = new ActionTask();
                    task.timer = 2;
                    brain.reaction = CurrentAction.reaction;
                    Play(task);
                }
            };
            brain.AddReaction(SeeBone, EatBone);
            brain.AddReaction(SeeCannibal, Bark);
            brain.AddReaction(SeeCorpse, Bark);
            brain.AddReaction(SeeBuisson, Bark);
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
            GameObject target = (CurrentAction.callData as SightInfo).target;
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(target.transform.position, barkDistance))
                {
                    LookAt(target.transform.position);
                    animator.Play("Bark");
                    if (target.GetComponent<Bush>() != null)
                        hunter.Call(target, true);
                    else
                        hunter.Call(target);
                    Stop();
                    Wait(1);
                }
            };
            action.AddReaction(
                () => los.sighted.Find(x => x.target == target) == null,
                () => Stop());
            Play(action);
        }


        void EatBone()
        {
            Bone target = (CurrentAction.callData as Bone);
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(target.transform.position, 2))
                {
                    LookAt(target.transform.position);
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

        bool SeeBone()
        {
            SightInfo bone = los.FindNearest(x => {
                if (x.target == null) return false;
                Bone b = x.target.GetComponent<Bone>();
                return b != null && b.linkedCannibal == null;
                });
            if (bone != null)
            {
                CurrentAction.callData = bone.target.GetComponent<Bone>();
                return true;
            }
            return false;
        }

        bool SeeCannibal()
        {
            SightInfo cannibal = los.FindNearest(x =>
            {
                Cannibal c = x.target.GetComponentInParent<Cannibal>();
                return c != null && !c.IsDead() && (Chasseur.alert || c.isCoverOfBlood);
            });
            if (cannibal != null)
            {
                if (los.getDetectRate(cannibal) == 1)
                    currentTarget = cannibal.target;
                else
                    currentTarget = null;
                CurrentAction.callData = cannibal;
                return true;
            }
            return false;
        }

        bool SeeCorpse()
        {
            SightInfo corpse = los.FindNearest(x =>
            {
                Corpse c = x.target.GetComponent<Corpse>();
                return c != null;
            });
            SightInfo can = los.FindNearest(x =>
            {
                Cannibal c = x.target.GetComponentInParent<Cannibal>();
                return c != null;
            });
            if(corpse != null && can != null)
            {
                CurrentAction.callData = can;
                return true;
            }
            return false;
        }

        bool SeeBuisson()
        {
            SightInfo buisson = los.FindNearest(x => {
                Bush b = x.target.GetComponent<Bush>();
                return b != null && b.IsMoving();
            });
            if (buisson != null)
            {
                CurrentAction.callData = buisson;
                return true;
            }
            return false;
        }


    }

}
