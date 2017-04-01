using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_LaunchObject : Cannibal_State {

    public Vector3 force;

    protected override void OnEnter()
    {
        base.OnEnter();
        Vector3 realForce = cannibal.value.m_cannibalAppearence.m_appearenceTransform.forward;
        realForce += force;
        ((IDropable)cannibal.value.m_cannibalSkill.m_cannibalObject).Throw(realForce);
        LooseCannibalObject();
    }

}
