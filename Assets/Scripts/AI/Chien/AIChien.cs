using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIChien : AIAgent {

        public Chasseur hunter;
        
        LineOfSight los;
        public enum btargetType
        {
            Viande, Buisson, Animal
        }
        

        enum DogTask
        {
            WanderInFront,
            ChaseAndBark,
            Eat
        }

        public Animator animator;
        int anim_speedId = Animator.StringToHash("Speed");


        // Use this for initialization
        new void Start () {
            base.Start();
            type = AIType.Dog;
            los = GetComponent<LineOfSight>();
            AkSoundEngine.PostEvent("dog_idle", gameObject);
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


            switch (CurrentTask.id)
            {
                case (int)DogTask.WanderInFront:
                    WanderAround(hunter.transform.position + hunter.transform.forward * 5, 3);
                    break;
            }

	    }


        void AnalyseSight()
        {
            for(int i = 0; i < los.sighted.Count; i++)
            {
                if (los.sighted[i].GetComponent<Bone>() != null)
                {
                    //target = los.sighted[i];
                    //targetType = btargetType.Viande;
                    //sawSomething = true;
                
                }
                else
                {
                    Bush buisson = los.sighted[i].GetComponent<Bush>();
                    if(buisson != null && buisson.IsMoving())
                    {
                        //target = los.sighted[i];
                        //targetType = btargetType.Buisson;
                        //sawSomething = true;
                    }
                }
            }
        }
    }

}
