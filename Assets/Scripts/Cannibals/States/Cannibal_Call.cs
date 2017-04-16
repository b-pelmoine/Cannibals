public class Cannibal_Call : Cannibal_State {

    protected override void OnEnter()
    {
        base.OnEnter();
        ((ICall)m_cannibal.m_cannibalSkill.m_cannibalObject).Use();
    }

}
