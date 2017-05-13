using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using UnityEngine;

[Category("Cannibal")]
public class Cannibal_Run : ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_cannibal.value.m_cannibalMovement.GroundMove(new Vector2(m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("SideMove"), m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("FrontMove")));

        Vector3 orientationDirection = m_cannibal.value.m_cannibalMovement.CharacterControllerEx.velocity;
        orientationDirection.y = 0;
        m_cannibal.value.m_cannibalAppearence.Orientate(orientationDirection);
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
