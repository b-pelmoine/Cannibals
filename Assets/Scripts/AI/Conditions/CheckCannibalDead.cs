using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Name("Check if Cannibal is Dead")]
[Category("Cannibal")]
public class CheckCannibalDead : ConditionTask
{
    public BBParameter<GameObject> target;

    protected override bool OnCheck()
    {
        Cannibal can = target.value.GetComponentInParent<Cannibal>();
        if (can != null && can.IsDead())
            return true;
        else
            return false;
    }

    protected override string info
    {
        get
        {
            return "Cannibal is dead";
        }
    }

}