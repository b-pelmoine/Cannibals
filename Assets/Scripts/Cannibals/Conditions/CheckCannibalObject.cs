using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class CheckCannibalObject : ConditionTask<Cannibal> {

    public System.Type type;

    protected override bool OnCheck()
    {
        return agent.m_cannibalSkill.m_cannibalObject && agent.m_cannibalSkill.m_cannibalObject.GetType().GetInterface(type.Name) != null;
    }

}
