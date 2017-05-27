using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using UnityEngine;

[Category("Cannibal")]
public class Cannibal_RunCorpse :ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;
    public float speedRunSoloFactor = 0.33333333333f;
    public float speedRunDuoFactor = 0.5f;

    protected override void OnEnter()
    {
        m_cannibal.value.m_cannibalSkill.m_corpse.Move(true);
        base.OnEnter();
    }
    protected override void OnExit()
    {
        base.OnExit();
        m_cannibal.value.m_cannibalMovement.ResetMaxSpeed();
        m_cannibal.value.m_cannibalSkill.m_corpse.Move(false);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if(m_cannibal.value.m_cannibalSkill.m_corpse.cannibals.Count == 1)
            m_cannibal.value.m_cannibalMovement.m_currentMaxRunSpeed = m_cannibal.value.m_cannibalMovement.MaxRunSpeed * speedRunSoloFactor;
        else
            m_cannibal.value.m_cannibalMovement.m_currentMaxRunSpeed = m_cannibal.value.m_cannibalMovement.MaxRunSpeed * speedRunDuoFactor;

        m_cannibal.value.m_cannibalMovement.GroundMove(new Vector2(m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("SideMove"), m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("FrontMove")).normalized);
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
    public bool Kill() { m_cannibal.value.m_cannibalSkill.ReleaseCorpse(); this.FSM.SendEvent("Death"); return true; }

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
