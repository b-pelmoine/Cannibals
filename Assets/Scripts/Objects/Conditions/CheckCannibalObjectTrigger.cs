using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckCannibalObjectTrigger : CheckTriggerExt<CannibalObject> {

    public BBParameter<Cannibal> m_cannibal;

    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
        {
            for(int i = savedList.value.Count -1; i >=0; i--)
            {
                if (savedList.value[i].linkedCannibal == m_cannibal.value)
                    savedList.value.RemoveAt(i);
                else
                    savedList.value[i].ShowIcon();
            }
        }

        return savedList.value.Count > 0;
    }
}
