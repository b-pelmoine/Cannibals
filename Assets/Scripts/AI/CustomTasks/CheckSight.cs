using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSight : ConditionTask {
    public BBParameter<List<GameObject>> sighted;
    public BBParameter<GameObject> target;
    public BBParameter<string> tag;

    protected override bool OnCheck()
    {
        foreach(GameObject obj in sighted.value)
        {
            if (obj.CompareTag(tag.value))
            {
                target.value = obj;
                return true;
            }
        }
        return false;
    }

    protected override string info
    {
        get
        {
            return "See "+tag;
        }
    }

}
