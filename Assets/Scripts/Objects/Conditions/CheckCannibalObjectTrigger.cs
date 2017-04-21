using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckCannibalObjectTrigger : CheckTrigger {



    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
        {
            CannibalObject cannibalObject = saveGameObjectAs.value.GetComponentInParent<CannibalObject>();

            if (cannibalObject)
            {
                cannibalObject.ShowIcon();
            }
        }

        return check;
    }
}
