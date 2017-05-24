using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCorpseBoxOverlap : CheckBoxOverlapCondition<Corpse> {

    public int cannibalCount;

    protected override void SpecialCondition(List<Corpse> savedList)
    {
        for (int i = savedList.Count - 1; i >= 0; i--)
        {
            if (savedList[i].cannibals.Count != cannibalCount || (savedList[i].cannibals.Count > 0 && savedList[i].cannibals[0].IsTakingCorpse()))
            {
                savedList.RemoveAt(i);
            }
            else
                savedList[i].ShowTakeIcon();
        }
    }

}
