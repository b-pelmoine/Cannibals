using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCannibalsBoxOverlap : CheckBoxOverlapCondition<Cannibal> {

    protected override void SpecialCondition(List<Cannibal> detecteds)
    {
        for(int i = savedList.value.Count -1; i >=0; i--)
        {
            if (savedList.value[i].IsDead())
            {
                savedList.value[i].m_cannibalAppearence.ShowResurrectIcon();
            }
            else
            {
                savedList.value.RemoveAt(i);
            }
        }
    }

}
