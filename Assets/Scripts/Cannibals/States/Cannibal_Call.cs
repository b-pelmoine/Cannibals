public class Cannibal_Call : Cannibal_State {

    protected override void OnEnter()
    {
        base.OnEnter();
        cannibal.value.m_cannibalSkill.m_call.Use();
    }

}
