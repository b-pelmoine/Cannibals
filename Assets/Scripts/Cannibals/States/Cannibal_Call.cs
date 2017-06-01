using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_Call : ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_cannibal.value.m_cannibalSkill.m_cannibalObject)
            ((ICall)m_cannibal.value.m_cannibalSkill.m_cannibalObject).Use();
        else
            this.Finish();
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
