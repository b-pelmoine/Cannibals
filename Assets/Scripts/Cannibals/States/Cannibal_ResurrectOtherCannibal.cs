using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_ResurrectOtherCannibal : Cannibal_State {

    public BBParameter<List<Cannibal>> cannibals;

    protected override void OnEnter()
    {
        base.OnEnter();
        cannibals.value[0].Resurrect();
    }

}
