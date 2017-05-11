using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_DualTakeCorpse : ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;
    public BBParameter<List<Corpse>> corpses;

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.value.m_cannibalSkill.TakeCorpse(corpses.value[0]);
    }

    /// <summary>
    /// Resuscitate the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be resuscitate for the moment</returns>
    public bool Resurrect() { return false; }

    /// <summary>
    /// Kill the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be killed in the current state</returns>
    public bool Kill() { m_cannibal.value.m_cannibalSkill.ReleaseCorpse(); this.FSM.SendEvent("Death"); return true; }

    /// <summary>
    /// Return if in the currentState the cannibal is considered dead
    /// </summary>
    /// <returns></returns>
    public bool IsDead() { return false; }
}
