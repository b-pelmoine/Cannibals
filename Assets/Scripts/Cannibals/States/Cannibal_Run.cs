using UnityEngine;

public class Cannibal_Run : Cannibal_State {


    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_cannibal.m_cannibalMovement.GroundMove(new Vector2(m_cannibal.m_rewiredInput.m_playerInput.GetAxis("SideMove"), m_cannibal.m_rewiredInput.m_playerInput.GetAxis("FrontMove")));

        Vector3 orientationDirection = m_cannibal.m_cannibalMovement.CharacterControllerEx.velocity;
        orientationDirection.y = 0;
        m_cannibal.m_cannibalAppearence.Orientate(orientationDirection);
    }

}
