using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_SoloTakeCorpse : Cannibal_State {

    public  BBParameter<List<Corpse>> corpses;

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.m_cannibalSkill.TakeCorpse(corpses.value[0]);
    }

}
