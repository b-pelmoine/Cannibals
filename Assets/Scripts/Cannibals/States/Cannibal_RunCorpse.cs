using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using UnityEngine;

[Category("Cannibal")]
public class Cannibal_RunCorpse :ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_cannibal.value.m_cannibalSkill.m_corpse.cannibals.Count <= 1)
            return;

        Vector2 globalInput = Vector3.zero;

        foreach (Cannibal c in m_cannibal.value.m_cannibalSkill.m_corpse.cannibals)
        {
           globalInput = new Vector2(globalInput.x + c.m_rewiredInput.m_playerInput.GetAxis("SideMove"),globalInput.y + +c.m_rewiredInput.m_playerInput.GetAxis("FrontMove"));
        }

        globalInput /= m_cannibal.value.m_cannibalSkill.m_corpse.cannibals.Count;

        foreach (Cannibal c in m_cannibal.value.m_cannibalSkill.m_corpse.cannibals)
            c.m_cannibalMovement.GroundMove(globalInput);

        if (m_cannibal.value.m_cannibalMovement.CharacterControllerEx.velocity.magnitude != 0)
        {
            Vector3 orientationDirection = m_cannibal.value.m_cannibalMovement.CharacterControllerEx.velocity;
            orientationDirection.y = 0;
            m_cannibal.value.m_cannibalAppearence.Orientate(orientationDirection);
        }
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
}
