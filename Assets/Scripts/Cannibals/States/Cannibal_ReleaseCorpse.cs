using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_ReleaseCorpse : Cannibal_State {

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.m_cannibalSkill.ReleaseCorpse();
    }
}
