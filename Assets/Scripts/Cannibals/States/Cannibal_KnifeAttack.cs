using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_KnifeAttack :ActionState, ICannibal_State
{
    public BBParameter<Cannibal> m_cannibal;
    public BBParameter<List<IKnifeKillable>> iKillables;

    protected override void OnEnter()
    {
        base.OnEnter();
        iKillables.value[0].KnifeKill();
        m_cannibal.value.m_cannibalAppearence.ShowKnife();
    }


    protected override void OnExit()
    {
        base.OnExit();
        m_cannibal.value.m_cannibalAppearence.HideKnife();
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
