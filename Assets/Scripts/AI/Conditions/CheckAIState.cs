using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Name("Check State of AI")]
[Category("AI")]
public class CheckAIState : ConditionTask {
    public bool useThis = true;
    public BBParameter<GameObject> target;
    public BBParameter<AI.AIAgent.AIState> state;

    protected override bool OnCheck()
    {
        AI.AIAgent ai;
        if (useThis)
        {
            ai = agent.GetComponent<AI.AIAgent>();
        }
        else {
            ai = target.value.GetComponent<AI.AIAgent>();
        }
        if (ai!=null && ai.State == state.value)
            return true;
        return false;
    }

    protected override string info
    {
        get
        {
            return "State ==" + state.value;
        }
    }

}
