using ParadoxNotion.Design;
using NodeCanvas.Framework;

[Category("Rewired")]
public class RewiredButton : ConditionTask<RewiredInput>
{
    public enum MODE { DOWN, UP, PRESS }

    public string buttonID;
    public MODE mode;

    protected override bool OnCheck()
    {
        switch (mode)
        {
            case MODE.DOWN: return agent.m_playerInput.GetButtonDown(buttonID);
            case MODE.UP: return agent.m_playerInput.GetButtonUp(buttonID);
            case MODE.PRESS: return agent.m_playerInput.GetButton(buttonID);
        }

        return false;
    }
}
