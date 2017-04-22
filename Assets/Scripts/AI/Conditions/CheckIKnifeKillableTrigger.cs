using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckIKnifeKillableTrigger : CheckTriggerExt<IKnifeKillable>
{
    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
        {
            foreach (IKnifeKillable k in savedList.value)
            {
                if (k != null && k.IsKnifeVulnerable())
                {
                    k.ShowKnifeIcon();
                    return true;
                }
            }
        }

        return check;
    }
}