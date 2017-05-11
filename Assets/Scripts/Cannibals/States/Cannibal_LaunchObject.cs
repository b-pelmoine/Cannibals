using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_LaunchObject : ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;

    public float force = 5;

    protected override void OnEnter()
    {
        base.OnEnter();
        ((IDropable)m_cannibal.value.m_cannibalSkill.m_cannibalObject).Throw(force, m_cannibal.value.m_cannibalAppearence.m_appearenceTransform.forward);
        m_cannibal.value.m_cannibalSkill.LooseCannibalObject();
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
