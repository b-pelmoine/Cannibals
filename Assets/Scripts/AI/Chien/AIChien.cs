using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChien : MonoBehaviour {

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

    public Animator animator;
    NavMeshAgent agent;
    int anim_speedId = Animator.StringToHash("Speed");

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        los = GetComponent<LineOfSight>();
	}
	
	// Update is called once per frame
	void Update () {
        if (animator != null && agent != null)
        {
            animator.SetFloat(anim_speedId, agent.velocity.magnitude);
        }
        if (los!=null && !sawSomething && los.Updated)
        {
            AnalyseSight();
        }
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
