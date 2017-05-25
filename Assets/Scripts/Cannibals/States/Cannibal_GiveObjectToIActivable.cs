using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_GiveObjectToIActivable : ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;
    public BBParameter<List<IActivable>> iactivables;

    protected override void OnEnter()
    {
        base.OnEnter();

        iactivables.value[0].Activate(m_cannibal.value.m_cannibalSkill.m_cannibalObject);

        if(m_cannibal.value.m_cannibalSkill.m_cannibalObject)
             m_cannibal.value.LooseCannibalObject();
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


    /// <summary>
    /// Return if in the currentState the cannibal is taking a corpse (not having a corpse, just tkaing it from the ground ! )
    /// </summary>
    /// <returns></returns>
    public bool IsTakingCorpse() { return false; }

}
