using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_TakeObject : Cannibal_State
{

    public BBParameter<List<CannibalObject>> cannibalObjects;

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.m_cannibalSkill.TakeCannibalObject(cannibalObjects.value[0]);
    }

}
