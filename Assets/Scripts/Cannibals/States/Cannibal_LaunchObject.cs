using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_LaunchObject : Cannibal_State {

    public float force = 5;

    protected override void OnEnter()
    {
        base.OnEnter();
        ((IDropable)cannibal.value.m_cannibalSkill.m_cannibalObject).Throw(force, cannibal.value.m_cannibalAppearence.m_appearenceTransform.forward);
        LooseCannibalObject();
    }

}
