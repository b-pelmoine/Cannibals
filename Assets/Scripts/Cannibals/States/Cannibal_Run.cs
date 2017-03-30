using UnityEngine;

public class Cannibal_Run : Cannibal_State {


    protected override void OnUpdate()
    {
        base.OnUpdate();
        cannibal.value.m_cannibalMovement.GroundMove(new Vector2(cannibal.value.m_rewiredInput.m_playerInput.GetAxis("SideMove"), cannibal.value.m_rewiredInput.m_playerInput.GetAxis("FrontMove")));
    }

}
