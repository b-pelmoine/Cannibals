using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using System.Collections.Generic;

[Category("Cannibal")]
public class Cannibal_TakeObject : ActionState, ICannibal_State
{
    public BBParameter<Cannibal> m_cannibal;
    public BBParameter<List<CannibalObject>> cannibalObjects;

    protected override void OnEnter()
    {
        if (cannibalObjects.value[0])
            m_cannibal.value.m_cannibalSkill.TakeCannibalObject(cannibalObjects.value[0]);
        else
            this.Finish();

        base.OnEnter();
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
