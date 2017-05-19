using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIChien : AIAgent {

        public Chasseur hunter;
        

        public enum btargetType
        {
            Viande, Buisson, Animal
        }
        

        enum DogTask
        {
            WanderInFront,
            ChaseAndBark,
            Eat,
            Look
        }

        public Animator animator;
        int anim_speedId = Animator.StringToHash("Speed");

        public float eatingTime = 3;
        public float stopTime = 3;
        public float wanderDistance = 3;
        public float barkDistance = 7;



        // Use this for initialization
        new void Start () {
            base.Start();
            type = AIType.Dog;
            //AkSoundEngine.PostEvent("dog_idle", gameObject);
            tasks.Push(new Task((int)DogTask.WanderInFront));
        }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
            if (animator != null && agent != null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            if (los!=null && los.Updated)
            {
                AnalyseSight();
            }

            Vector3 distance;

            switch (CurrentTask.id)
            {
                case (int)DogTask.Look:
                    if (Look())
                    {
                        GameObject target = CurrentTask.target;
                        tasks.Pop();
                        tasks.Push(new Task((int)DogTask.ChaseAndBark, target));
                    }
                    break;
                case (int)DogTask.WanderInFront:
                    if (CurrentTask.count == 0)
                    {
                        if (WanderAround(hunter.transform.position + hunter.transform.forward * 0, wanderDistance))
                        {
                            CurrentTask.elapsed = 0;
                            CurrentTask.count++;
                            //AkSoundEngine.PostEvent("dog_idle", gameObject);
                        }
                    }
                    else
                    {
                        if (CurrentTask.elapsed > stopTime)
                        {
                            AkSoundEngine.PostEvent("dog_sniff", gameObject);
                            CurrentTask.count = 0;
                        }
                    }
                    break;

                case (int)DogTask.ChaseAndBark:
                    if(MoveTo(CurrentTask.target.transform.position, barkDistance))
                    {
                        if (CurrentTask.count == 0)
                        {
                            Vector3 targetLookAt = CurrentTask.target.transform.position;
                            targetLookAt.y = agent.transform.position.y;
                            agent.transform.LookAt(targetLookAt);
                            animator.Play("Bark");
                            hunter.Call(CurrentTask.target);
                            
                            //AkSoundEngine.PostEvent("dog_bark", gameObject);
                            CurrentTask.count++;
                        }
                        else if(CurrentTask.elapsed>2)
                        {
                            tasks.Pop();
                        }
                        
                    }
                    else
                    {
                        CurrentTask.count = 0;
                    }
                    distance = CurrentTask.target.transform.position - transform.position;
                    distance.y = 0;
                    if (distance.sqrMagnitude > Mathf.Pow(los.radius, 2) || CurrentTask.elapsed > 10)
                    {
                        tasks.Pop();
                    }
                    break;

                case (int)DogTask.Eat:
                    if(CurrentTask.count==0 && MoveTo(CurrentTask.target.transform.position, 3))
                    {
                        animator.Play("IdleToEat");
                        //AkSoundEngine.PostEvent("dog_eat", gameObject);
                        CurrentTask.count = 1;
                        CurrentTask.elapsed = 0;
                    }
                    else if (CurrentTask.count == 1 && CurrentTask.elapsed>eatingTime)
                    {
                        CurrentTask.target.gameObject.SetActive(false);
                        animator.Play("Idle");
                        tasks.Pop();
                    }
                    break;
            }

	    }


        void AnalyseSight()
        {
            foreach(SightInfo obj in los.sighted)
            {
                if (CurrentTask.id != (int)DogTask.Eat && obj.target.GetComponent<Bone>() != null)
                {
                    //target = los.sighted[i];
                    //targetType = btargetType.Viande;
                    //sawSomething = true;
                    tasks.Push(new Task((int)DogTask.Eat, obj.target));
                
                }
                else if (CurrentTask.id == (int)DogTask.WanderInFront && obj.target.CompareTag("Player"))
                {
                    Cannibal can = obj.target.GetComponentInParent<Cannibal>();
                    if(can!=null && !can.IsDead())
                        tasks.Push(new Task((int)DogTask.Look, obj.target));
                }
                else
                {
                    Bush buisson = obj.target.GetComponent<Bush>();
                    if(CurrentTask.id == (int)DogTask.WanderInFront && buisson != null && buisson.IsMoving())
                    {
                        tasks.Push(new Task((int)DogTask.ChaseAndBark, obj.target));
                        //target = los.sighted[i];
                        //targetType = btargetType.Buisson;
                        //sawSomething = true;
                    }
                }
            }
        }
    }

}
