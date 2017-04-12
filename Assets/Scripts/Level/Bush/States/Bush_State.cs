using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Bush")]
public abstract class Bush_State : ActionState {

    public BBParameter<Bush> bush;

    protected Bush m_bush;

    protected override void OnInit()
    {
        base.OnInit();
        m_bush = bush.value;
    }

    public virtual bool Move()
    {
        FSM.SendEvent("Move");
        return true;
    }

    public virtual bool IsMoving()
    {
        return false;
    }
}
