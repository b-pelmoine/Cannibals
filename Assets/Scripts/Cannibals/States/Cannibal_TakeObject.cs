﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_TakeObject : Cannibal_State
{

    public BBParameter<GameObject> cannibalObject;

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.m_cannibalSkill.TakeCannibalObject(cannibalObject.value.GetComponent<CannibalObject>());
    }

}
