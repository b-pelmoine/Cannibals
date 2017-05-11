using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_HunterEar : ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;

    protected override void OnEnter()
    {
        base.OnEnter();

        if (Cannibal_Skill.OnStartUseHunterSense != null)
            Cannibal_Skill.OnStartUseHunterSense(m_cannibal.value);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (Cannibal_Skill.OnStopUseHunterSense != null)
            Cannibal_Skill.OnStopUseHunterSense(m_cannibal.value);
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
    public bool Kill() { this.FSM.SendEvent("Death"); return true; }

    /// <summary>
    /// Return if in the currentState the cannibal is considered dead
    /// </summary>
    /// <returns></returns>
    public bool IsDead() { return false; }
}
