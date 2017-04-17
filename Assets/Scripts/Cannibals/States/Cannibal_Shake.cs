using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Shake : Cannibal_State
{
    protected override void OnUpdate()
    {
        base.OnUpdate();
        ((IShakable)m_cannibal.m_cannibalSkill.m_cannibalObject).Shake();
    }

}
