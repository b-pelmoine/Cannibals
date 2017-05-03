using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Dead : Cannibal_State {


    protected override void OnEnter()
    {
        base.OnEnter();
        if (m_cannibal.m_cannibalSkill.m_corpse != null)
        {
            m_cannibal.m_cannibalSkill.ReleaseCorpse();
        } 
    }

    /// <summary>
    /// Nothing to do if we try to kill a cannibal that is already dead
    /// </summary>
    /// <returns></returns>
    public override bool Kill()
    {
        return false;
    }

    public override bool Resurrect()
    {
        FSM.SendEvent("Resurrect");
        return true;
    }

    public override bool IsDead()
    {
        return true;
    }

}
