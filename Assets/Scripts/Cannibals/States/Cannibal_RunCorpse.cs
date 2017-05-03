using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_RunCorpse : Cannibal_State {

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_cannibal.m_cannibalSkill.m_corpse.cannibals.Count <= 1)
            return;

        Vector2 globalInput = Vector3.zero;

        foreach (Cannibal c in m_cannibal.m_cannibalSkill.m_corpse.cannibals)
        {
           globalInput = new Vector2(globalInput.x + c.m_rewiredInput.m_playerInput.GetAxis("SideMove"),globalInput.y + +c.m_rewiredInput.m_playerInput.GetAxis("FrontMove"));
        }

        globalInput /= m_cannibal.m_cannibalSkill.m_corpse.cannibals.Count;

        foreach (Cannibal c in m_cannibal.m_cannibalSkill.m_corpse.cannibals)
            c.m_cannibalMovement.GroundMove(globalInput);

        if (m_cannibal.m_cannibalMovement.CharacterControllerEx.velocity.magnitude != 0)
        {
            Vector3 orientationDirection = m_cannibal.m_cannibalMovement.CharacterControllerEx.velocity;
            orientationDirection.y = 0;
            m_cannibal.m_cannibalAppearence.Orientate(orientationDirection);
        }
    }

}
