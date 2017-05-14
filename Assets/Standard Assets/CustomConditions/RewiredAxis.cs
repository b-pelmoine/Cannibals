using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Rewired")]
public class RewiredAxis : ConditionTask<RewiredInput>
{
    public string[] axisIDs;

    public float minValue;

    protected override bool OnCheck()
    {
        float total = 0;

        for (int i = 0; i < axisIDs.Length; i++)
        {
            float axis = agent.m_playerInput.GetAxis(axisIDs[i]);
            total += axis * axis;
        }

        return Mathf.Pow(total,1f/ axisIDs.Length) >= minValue;
    }

}
