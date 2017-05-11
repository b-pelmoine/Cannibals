using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_HunterEar : Cannibal_State {

    protected override void OnEnter()
    {
        base.OnEnter();
        RewiredInput input_t = m_cannibal.gameObject.GetComponent<RewiredInput>();
        m_cannibal.m_cannibalSkill.TriggerHuntSense(input_t.playerInputID + input_t.number, true);
    }

    protected override void OnExit()
    {
        base.OnExit();
        RewiredInput input_t = m_cannibal.gameObject.GetComponent<RewiredInput>();
        m_cannibal.m_cannibalSkill.TriggerHuntSense(input_t.playerInputID + input_t.number, false);
    }
}
