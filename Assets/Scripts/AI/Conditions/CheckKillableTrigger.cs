using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckKillableTrigger : CheckTrigger
{
    public BBParameter<iKillable> killable;

    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check && saveGameObjectAs.value)
        {
            iKillable k = saveGameObjectAs.value.GetComponentInParent<iKillable>();

            if (k != null && k.IsVulnerable())
            {
                killable.value = k;
                k.ShowKnifeIcon();
                return true;
            }
            else
                return false;
        }

        return false;
    }
}