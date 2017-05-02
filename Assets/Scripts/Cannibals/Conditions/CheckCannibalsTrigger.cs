using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCannibalsTrigger : CheckTriggerExt<Cannibal> {

    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
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


        return savedList.value.Count > 0;
    }

}
