using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckState : ConditionTask
{
    public BBParameter<GameObject> target;
    public BBParameter<string> state;

    protected override bool OnCheck()
    {
        FSMOwner owner = target.value.GetComponentInParent<FSMOwner>();
        if (owner == null || owner.currentStateName != state.value)
            return false;
        return true;
    }

    protected override string info
    {
        get
        {
            return "State ==" + state.value;
        }
    }
}
