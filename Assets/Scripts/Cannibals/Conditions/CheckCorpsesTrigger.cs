using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCorpsesTrigger : CheckTriggerExt<Corpse> {

    public int cannibalCount;

    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
        {
            for (int i = savedList.value.Count - 1; i >= 0; i--)
            {
                if (savedList.value[i].cannibals.Count != cannibalCount || (savedList.value[i].cannibals.Count > 0 && savedList.value[i].cannibals[0].IsTakingCorpse()))
                {
                    savedList.value.RemoveAt(i);
                }
                else
                    savedList.value[i].ShowTakeIcon();
            }
        }

        return savedList.value.Count > 0;
    }

}
