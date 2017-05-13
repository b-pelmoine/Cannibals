using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

[Category("Cannibal")]
public class Cannibal_IdleCorpse : ActionState, ICannibal_State
{
    public BBParameter<Cannibal> m_cannibal;

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
