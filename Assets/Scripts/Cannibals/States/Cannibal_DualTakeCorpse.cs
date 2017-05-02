using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_DualTakeCorpse : Cannibal_State {


    public BBParameter<List<Corpse>> corpses;
    public BBParameter<Corpse> corpseTaken;

    protected override void OnEnter()
    {
        corpseTaken.value = corpses.value[0];
        corpseTaken.value.cannibals.Add(m_cannibal);
    }
}
