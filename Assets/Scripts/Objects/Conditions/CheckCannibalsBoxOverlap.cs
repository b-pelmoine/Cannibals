using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckCannibalObjectBoxOverlapCondition : CheckBoxOverlapCondition<CannibalObject> {

    public BBParameter<Cannibal> m_cannibal;

    protected override void SpecialCondition(List<CannibalObject> detecteds)
    {
        for(int i = savedList.value.Count -1; i >=0; i--)
        {
            if (savedList.value[i].linkedCannibal == m_cannibal.value)
                savedList.value.RemoveAt(i);
            else
                savedList.value[i].ShowIcon();
        }
    }
}
