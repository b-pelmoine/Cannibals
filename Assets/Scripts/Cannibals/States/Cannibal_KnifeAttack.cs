using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_KnifeAttack : Cannibal_State
{
    public BBParameter<List<IKnifeKillable>> iKillables;


    protected override void OnEnter()
    {
        base.OnEnter();
        iKillables.value[0].KnifeKill();
    }
}
