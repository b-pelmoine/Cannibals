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
                if(savedList.value[i].cannibals.Count != cannibalCount)
                {
                    savedList.value.RemoveAt(i);
                }
            }
        }

        return savedList.value.Count > 0;
    }

}
