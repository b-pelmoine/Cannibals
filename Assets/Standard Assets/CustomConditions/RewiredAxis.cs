using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Rewired")]
public class RewiredAxis : ConditionTask<RewiredInput>
{
    public string axisID;
    public float minValue;

    protected override bool OnCheck()
    {
        return Mathf.Abs(agent.m_playerInput.GetAxis(axisID)) >= minValue;
    }

}
