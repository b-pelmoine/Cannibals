using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChien : AI.AIAgent {

    IBlackboard blackboard;
    public bool arrived=false;
    LineOfSight los;
    public enum btargetType
    {
        Viande, Buisson, Animal
    }
    public btargetType targetType;
    public GameObject target = null;
    public bool sawSomething = false;

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
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        if (animator != null && agent != null)
        {
            animator.SetFloat(anim_speedId, agent.velocity.magnitude);
        }
        if (los!=null && !sawSomething && los.Updated)
        {
            AnalyseSight();
        }
        //if(target!=null)
          //  MoveTo(target.transform.position, 5);
	}


    void AnalyseSight()
    {
        for(int i = 0; i < los.sighted.Count; i++)
        {
            if (los.sighted[i].GetComponent<Bone>() != null)
            {
                target = los.sighted[i];
                targetType = btargetType.Viande;
                sawSomething = true;
                
            }
            else
            {
                Bush buisson = los.sighted[i].GetComponent<Bush>();
                if(buisson != null && buisson.IsMoving())
                {
                    target = los.sighted[i];
                    targetType = btargetType.Buisson;
                    sawSomething = true;
                }
            }
        }
    }
}
