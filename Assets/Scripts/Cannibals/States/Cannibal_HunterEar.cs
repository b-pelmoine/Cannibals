using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_HunterEar : Cannibal_State {

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.m_cannibalSkill.TriggerHuntSense(m_cannibal.gameObject.GetComponent<RewiredInput>().playerInputID, true);
    }

    protected override void OnExit()
    {
        base.OnExit();
        m_cannibal.m_cannibalSkill.TriggerHuntSense(m_cannibal.gameObject.GetComponent<RewiredInput>().playerInputID, false);
    }
}
