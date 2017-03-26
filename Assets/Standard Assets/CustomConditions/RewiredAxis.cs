using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using Rewired;
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
