using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Cannibal")]
public abstract class Cannibal_State : ActionState {

    public BBParameter<Cannibal> m_cannibal;


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
}
