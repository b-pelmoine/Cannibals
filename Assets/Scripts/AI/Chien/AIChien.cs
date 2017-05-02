using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	// Use this for initialization
	void Start () {
        los = GetComponent<LineOfSight>();
	}
	
	// Update is called once per frame
	void Update () {
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
