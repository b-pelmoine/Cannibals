using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckAITrigger : CheckTrigger
{
    public BBParameter<iAI> savedAI;

    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check && saveGameObjectAs.value)
        {
            iAI ai = saveGameObjectAs.value.GetComponentInParent<iAI>();

            if (ai != null && ai.IsVulnerable())
            {
                savedAI.value = ai;
                ai.ShowKnifeIcon();
                return true;
            }
            else
                return false;
        }

        return false;
    }
}