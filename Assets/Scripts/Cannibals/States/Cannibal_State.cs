using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Cannibal")]
public abstract class Cannibal_State : ActionState {

    public BBParameter<Cannibal> cannibal;

    protected Cannibal m_cannibal;

    protected override void OnInit()
    {
        base.OnInit();
        m_cannibal = cannibal.value;
    }


    /// <summary>
    /// Knock out the cannibal.
    /// </summary>
    /// <returns>false if the cannibal can't be knock out for the moment</returns>
    public virtual bool KnockOut() { return false; }

    /// <summary>
    /// Resuscitate the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be resuscitate for the moment</returns>
    public virtual bool Resuscitate() { return false; }

    /// <summary>
    /// Loose the CannibalObject
    /// </summary>
    public virtual bool LooseCannibalObject() {cannibal.value.m_cannibalSkill.LooseCannibalObject(); return true; }


    /// <summary>
    /// Kill the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be killed in the current state</returns>
    public virtual bool Kill() { this.FSM.SendEvent("Death"); return true; }

    /// <summary>
    /// Revive the Cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be revived in the current state</returns>
    public virtual bool Revive() { this.FSM.SendEvent("Revive"); return true; }
}
