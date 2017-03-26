using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Run : Cannibal_State {


    protected override void OnUpdate()
    {
        m_cannibal.value.CannibalMovement.GroundMove(new Vector2(m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("SideMove"), m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("FrontMove")));
    }

}
