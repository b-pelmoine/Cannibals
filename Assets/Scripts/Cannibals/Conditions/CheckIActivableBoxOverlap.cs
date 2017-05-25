using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class CheckIActivableBoxOverlap : CheckBoxOverlapCondition<IActivable> {

    public BBParameter<Cannibal> m_cannibal;

    protected override void SpecialCondition(List<IActivable> savedList)
    {
       for(int j = savedList.Count -1; j >=0; j--)
        {
            if (!savedList[j].IsActivable(m_cannibal.value.m_cannibalSkill.m_cannibalObject))
                savedList.RemoveAt(j);
            else
                savedList[j].ShowIcon();
        }
    }

}
