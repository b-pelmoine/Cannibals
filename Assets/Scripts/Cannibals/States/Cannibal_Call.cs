public class Cannibal_Call : Cannibal_State {

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.value.m_call.Use();
    }

}
